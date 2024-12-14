using Microsoft.AspNetCore.Mvc;
using TestController.Models;

namespace TestController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalizeController : ControllerBase
    {
        [HttpPost("GetAnalize")]
        public async Task<List<OutputAnalizeData>> GetAnalizeTest([FromBody] List<Employee> employees)
        {
            var output = await CreateOutputEmployeesOnActive(employees);
            
            return output;
        }

        /// <summary>
        /// Находит все дружеские пары и возвращает то что находится под пунктом OnActive
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        private async Task<List<OutputAnalizeData>> CreateOutputEmployeesOnActive(List<Employee> employees)
        {
            var friendCouples = await FindEmployeeFriendCouples(employees);
            
            var tasks = employees.Select(async employee =>
            {
                return new OutputAnalizeData
                {
                    Name = employee.Name,
                    Phone = await IsPhoneNumberOfEmployeeCorrectAsync(employee) ? employee.Phone : "Номер телефона содержит ошибки",
                    Email = await IsEmailOfEmployeeCorrect(employee) ? employee.Email : "Почта содержит ошибки",
                    NumberOfFriends = employee.Friends?.Count ?? 0,
                    EmployeeFriendCouples = friendCouples
                        .FirstOrDefault(e => e.Split(" - ")[0] == employee.Name)
                };
            });

            var outputAnalizeDataOfAllEmployers = (await Task.WhenAll(tasks)).ToList();

            var activeEmployees = employees.Where(row => row.IsActive).Select(a => a.Name).ToList();
            return outputAnalizeDataOfAllEmployers
                .Where(e => activeEmployees.Contains(e.Name!.Split(" - ")[0]))
                .ToList();
        }

        
        
        /// <summary>
        /// Метод проверяет по заготовленной маске что телефон введен верно
        /// предварительно проведя остальные проверки
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Верно ли написан номер</returns>
        private async Task<bool> IsPhoneNumberOfEmployeeCorrectAsync(Employee employee)
        {
            return await Task.Run(() =>
            {
                var phoneNumber = employee.Phone;
                if (string.IsNullOrWhiteSpace(phoneNumber)
                    || string.Concat(phoneNumber.Where(char.IsLetter)).Length > 0
                    || string.Concat(phoneNumber.Where(char.IsDigit)).Length != 11)
                {
                    return false;
                }

                const string mask = "+1 (XXX) XXX-XXXX";
                const string allowedChars = " ()-";
                
                var maskedNumber = new char[phoneNumber.Length];
                maskedNumber[0] = '+';
                maskedNumber[1] = '1';
                for (var i = 2; i < phoneNumber.Length; i++)
                {
                    if (allowedChars.Contains(phoneNumber[i]))
                    {
                        maskedNumber[i] = phoneNumber[i];
                    }
                    else
                    {
                        maskedNumber[i] = 'X';
                    }
                }

                return mask == string.Concat(maskedNumber);
            });
            
        }

        /// <summary>
        /// Здесь проводим проверки правильности написания электронной почты
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Верно ли написана почта</returns>
        private async Task<bool> IsEmailOfEmployeeCorrect(Employee employee)
        {
            return await Task.Run(() =>
            {
                var emailOfEmployee = employee.Email;
                if (string.IsNullOrWhiteSpace(emailOfEmployee)) return false;
                
                char[] notAllowedChars = { '+', '@', '#' , '!', '*', '&', '(',  ')' };
                const string domain = "@artiq.com";
                var atIndex = emailOfEmployee.IndexOf(domain, StringComparison.Ordinal);
                var prefix = emailOfEmployee.Substring(0, atIndex);
            
                return !prefix.Any(charOfPrefix => notAllowedChars.Any(charOfNotAllowedChars 
                           => charOfNotAllowedChars == charOfPrefix))
                       && emailOfEmployee.EndsWith("@artiq.com");
            });
            
        }
        

        /// <summary>
        /// Ищем дружеские пары сотрудников
        /// </summary>
        /// <returns>Возвращает список дружеских пар для каждого из сотрудников в виде dictionary</returns>
        private async Task<List<string>> FindEmployeeFriendCouples(List<Employee> employees)
        {
            return await Task.Run(() =>
            {
                var listOfEmployersFriendsCouples = new List<string>();

                // Пробегаемся по каждому из сотрудников
                foreach (var employee in employees)
                {
                    var isFind = false;

                    if (employee.Friends == null || employee.Friends.Count == 0)
                    {
                        listOfEmployersFriendsCouples.Add(employee.Name! + " - Нет друзей");
                        continue;
                    }

                    // Пробегаемся по листу друзей сотрудника
                    foreach (var friendOfEmployee in employee.Friends)
                    {
                        var friend = employees.FirstOrDefault(e => e.Name == friendOfEmployee.Name);

                        if (friend != null && friend.Friends != null)
                        {
                            // Проверяем, является ли этот друг другом сотрудника
                            if (friend.Friends.Any(f => f.Name == employee.Name))
                            {
                                var pair = employee.Name + " - " + friendOfEmployee.Name;

                                isFind = true;
                                listOfEmployersFriendsCouples.Add(pair);
                            }
                        }
                    }

                    if (!isFind) listOfEmployersFriendsCouples.Add($"{employee.Name} - Дружеские пары отсутствуют");
                }

                
                
                for (var i = 0; i < listOfEmployersFriendsCouples.Count; i++)
                {
                    var leftNameI = listOfEmployersFriendsCouples[i].Split(" - ")[0];
                    
                    
                    for (var j = 1 + i; j < listOfEmployersFriendsCouples.Count; j++)
                    {
                        var leftNameJ = listOfEmployersFriendsCouples[j].Split(" - ")[0];
                        
                        
                        if (i != j && leftNameI == leftNameJ)
                        {
                            listOfEmployersFriendsCouples[j] =
                                listOfEmployersFriendsCouples[i] + ", " + listOfEmployersFriendsCouples[j];

                            listOfEmployersFriendsCouples.RemoveAt(i);
                        }
                        
                    }
                    
                }
                
                
                return listOfEmployersFriendsCouples;
            });
        }
    }
}

// Для файлов используется библиотека fs для node.js, установлена на стенде
const fs = require('fs');


// Функция, которая обращается к серверу
// В аргумент функиции передаются значения типа объект
// Для запросов библиотека тоже подключена
function getAnalize(data) {
    const express = require('express'),
    app = express(),
    request = require('request');


    request.post(
        {
            url: 'http://localhost:5222/api/Analize/GetAnalize',
            body: JSON.stringify(data),
            headers: {
                "Content-type": "application/json; charset=UTF-8"
              }                   
        },
        (err, response, body) => {   
            
            if(body) {
                console.log(JSON.parse(body));
                createResponseFile(JSON.parse(body));     
            }  
        }
    );
}

function createResponseFile(data) {
    const filePath = './answer.txt';

    const answer = data.map(entry => {
        return `Имя: ${entry.name}\n
        Номер телефона: ${entry.phone}\n
        Почта: ${entry.email}\n
        Количество друзей: ${entry['number-of-friends']}\n
        Дружеские пары: ${entry['employee-friends-couples']}\n`;
    }).join('\n');

    fs.writeFile(filePath, answer, (err) => {
        if (err) {
            console.error('Ошибка при записи файла:', err);
        } else {
            console.log('Файл успешно создан/перезаписан:', filePath);
        }
    });
}

// Функция для начала программы
function root() {
     const rawData = fs.readFileSync('./test.json', 'utf-8');
     const jsonData = JSON.parse(rawData);
     
    getAnalize(jsonData); 

}

root();
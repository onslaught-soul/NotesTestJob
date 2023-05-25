using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleNodeCreator
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ValidateCertificate;

                Timer timer = new Timer(CreateRandomNote, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                Console.ReadLine();
                timer.Dispose();
            }
            catch (Exception ex) { Console.Write(ex.Message + "\n\rНажмите клавишу для завершения"); Console.ReadLine(); }
        }
        static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        static void CreateRandomNote(object state)
        {
            Task.Run(async () =>
            {
                using (HttpClient client = new HttpClient())
                {
                    var noteData = GenerateRandomNote();

                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("noteTitle", noteData.titles),
                        new KeyValuePair<string, string>("noteText", noteData.text)
                    });

                    HttpResponseMessage response = await client.PostAsync("http://localhost:80/api/notes/", formContent);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Заметка добавлена!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при добавлении заметки");
                    }
                }
            });
        }

        static (string titles, string text) GenerateRandomNote()
        {
            string[] titles = {
                "Важное событие",
                "Идея для проекта",
                "Список задач",
                "План на выходные",
                "Памятка по программированию",
                "Любимые цитаты",
                "Рецепт вкусного блюда",
                "Список покупок",
                "Советы по организации времени",
                "Путешествие мечты",
                "Книги для прочтения"
            };

            string[] texts = {
                "Не забыть отправить важный документ до конца дня.",
                "Идея для нового приложения: создать платформу для обмена книгами.",
                "Задачи на сегодня: сделать презентацию и отправить отчет.",
                "План на выходные: провести время с семьей и погулять на природе.",
                "Памятка по программированию: регулярные выражения и работа с базами данных.",
                "Любимые цитаты: \"Успех — это способность идти от одного неудачного проекта к другому, не теряя энтузиазма.\"",
                "Рецепт вкусного блюда: паста карбонара с беконом и пармезаном.",
                "Список покупок: молоко, яйца, овощи, фрукты.",
                "Советы по организации времени: планировать задачи и делать приоритеты.",
                "Место для путешествия мечты: Бали, Индонезия.",
                "Книги для прочтения: \"Мастер и Маргарита\", \"1984\", \"Анна Каренина\"."
            };

            Random random = new Random();
            int randomIndex = random.Next(0, titles.Length);

            return (titles[randomIndex], texts[randomIndex]);
        }
    }
}

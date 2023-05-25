using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Configuration;

namespace ConsoleNodeReader
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ValidateCertificate;

                Timer timer = new Timer(GetCheckNote, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                Console.ReadLine();
                timer.Dispose();
            }
            catch (Exception ex) { Console.Write(ex.Message + "\n\rНажмите клавишу для завершения"); Console.ReadLine(); }
        }
        static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        static void GetCheckNote(object state)
        {
            Task.Run(async () =>
            {
                using (HttpClient client = new HttpClient())
                {
                    
                    HttpResponseMessage response = await client.GetAsync("http://localhost:80/api/notes/check");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        List<POCO_Note> notes = JsonConvert.DeserializeObject<List<POCO_Note>>(json);
                        foreach (var note in notes)
                        {
                            Console.WriteLine("Note ID: " + note.Id);
                            Console.WriteLine("Note Title: " + note.Title);
                            Console.WriteLine("Note Text: " + note.Text);
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при выводе заметок!");
                    }
                }
            });
        }
        class POCO_Note
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
        }
    }
}

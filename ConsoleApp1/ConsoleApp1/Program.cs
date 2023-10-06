using Azure;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (EventsContext db = new EventsContext())
            {
                //Выполняем разные методы, содержащие операции выборки и изменения данных
                Console.WriteLine("====== Будет выполнена выборка данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Select(db, 5);
                Console.WriteLine("====== Будет выполнена вставка данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Insert(db);
                Console.WriteLine("====== Выборка после вставки ========");
                Select(db, 5);
                Console.WriteLine("====== Будет выполнено удаление данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Delete(db);
                Console.WriteLine("====== Выборка после удаления ========");
                Select(db, 5);
                Console.WriteLine("====== Будет выполнено обновление данных (нажмите любую клавишу) ========");
                Console.ReadKey();
                Update(db);
                Console.WriteLine("====== Выборка после обновления ========");
                Select(db, 5);
            }
        }

        static void Print(string sqltext, IEnumerable items)
        // Вывод на консоль
        {
            Console.WriteLine(sqltext);
            Console.WriteLine("Записи: ");
            foreach (var item in items)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine();
            Console.ReadKey();
        }

        static void Select(EventsContext db, int recordsNumber)
        {

            // Определение LINQ запроса 1
            var queryLINQ1 = from e in db.Enterprises
                             select new
                             {
                                 e.Id,
                                 e.Name,
                                 e.OwnershipForm,
                                 e.Adress,
                                 ManagerName = e.Manager.Name,
                                 ManagerSurname = e.Manager.Surname,
                                 ManagerMiddleName = e.Manager.MiddleName,
                                 ManagerPhone = e.Manager.Phone,
                                 CPETitle = e.CPE.Name,
                                 CPESurname = e.CPE.Surname,
                                 CPEMiddleName = e.CPE.MiddleName,
                                 CPEPhone = e.CPE.Phone
                             };

            string comment = "1. Результат выполнения запроса на выборку всех данных из таблицы Enterprises : \r\n";
            Print(comment, queryLINQ1.Take(recordsNumber).ToList());

            // Определение LINQ запроса 2 
            var queryLINQ2 = db.PlannedEvents
                .Where(pe => pe.Expenses > 50000) // фильтрация по Expenses больше 50000
                .Select(pe => new // проекция результата
                {
                    pe.Id,
                    pe.DateOfStart,
                    pe.DateOfEnd,
                    pe.Scope,
                    pe.Expenses,
                    pe.EconomicEffect,
                    EnterpriseName = pe.Enterprise.Name, // берем имя предприятия
                    Responsible = new // вместо ID ответственного выводим информацию о нем
                    {
                        pe.Responsible.Name,
                        pe.Responsible.Surname,
                        pe.Responsible.MiddleName,
                        pe.Responsible.Phone
                    },
                    EventName = pe.Event.Name, // берем имя события
                    FinanceSum = pe.Finance.Enterprise + pe.Finance.Organisation + pe.Finance.Ministry + pe.Finance.RepublicBudget + pe.Finance.LocalBudget // суммируем источники финансирования
                })
                .ToList();

            comment = "2. Результат выполнения запроса на выборку из таблицы PlannedEvents, где Expenses больше 50000 : \r\n";
            Print(comment, queryLINQ2.Take(recordsNumber).ToList());

            // Определение LINQ запроса 3
            var queryLINQ3 = from pe in db.PlannedEvents
                             group pe by pe.Enterprise into peGroup
                             select new
                             {
                                 EnterpriseName = peGroup.Key.Name,
                                 TotalExpenses = peGroup.Sum(x => x.Expenses)
                             };

            comment = "3. Результат выполнения запроса на выборку записей из таблицы PlannedEvents, сгруппированных по полю Enterprise с выводом суммарных вложений каждого предприятия: \r\n";
            Print(comment, queryLINQ3.Take(recordsNumber).ToList());
        }

        static void Insert(EventsContext db)
        {
            // Создать новое предприятие
            Enterprise enterprise = new Enterprise
            {
                Name = "Test_Enterprise",
                OwnershipForm = "test",
                Adress = "Test_Address",
                ManagerId = 1,
                CPEId = 1
            };
            // Спланировать новое мероприятие
            PlannedEvent plannedEvent = new PlannedEvent
            {
                DateOfStart = DateTime.Now,
                DateOfEnd = DateTime.Today,
                Expenses = 100000,
                EconomicEffect = 150000,
                EventId = 1,
                EnterpriseId = 1,
                EmployeeId = 1,
                FinanceId = 1
            };

            // Добавить в DbSet
            db.Enterprises.Add(enterprise);
            db.PlannedEvents.Add(plannedEvent);
            // Сохранить изменения в базе данных
            db.SaveChanges();
        }

        static void Delete(EventsContext db)
        {
            //подлежащие удалению записи в таблице Enterprises
            string enterpriseName = "Test_Enterprise";
            IQueryable<Enterprise> enterprise = db.Enterprises.Where(c => c.Name == enterpriseName);

            //подлежащие удалению записи в таблице PlannedEvents
            int eventId = 1;
            IQueryable<PlannedEvent> plannedEvent = db.PlannedEvents
                .Where(c => c.EventId == eventId);

            //Удаление записей в таблице Enterprises 
            db.Enterprises.RemoveRange(enterprise);
            // сохранить изменения в базе данных
            db.SaveChanges();

            //Удаление записей в таблице PlannedEvents
            db.PlannedEvents.RemoveRange(plannedEvent);
            // сохранить изменения в базе данных
            db.SaveChanges();
        }

        static void Update(EventsContext db)
        {
            //подлежащие обновлению записи в таблице Tanks
            string enterpriseName = "Тестирование процеду";
            Enterprise enterprise = db.Enterprises.Where(c => c.Name == enterpriseName).FirstOrDefault();
            //обновление
            if (enterprise != null)
            {
                enterprise.Name = "Тестирование";
                enterprise.Adress = "TEST";
            };
            // сохранить изменения в базе данных
            db.SaveChanges();

        }
    }
}


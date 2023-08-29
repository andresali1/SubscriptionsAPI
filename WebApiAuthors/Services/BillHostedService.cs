using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace WebApiAuthors.Services
{
    public class BillHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public BillHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(HandleBills, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Method that generate bills and set bad payment history when bills expire
        /// </summary>
        /// <param name="state"></param>
        private void HandleBills(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SetBadPaymentHistory(context);
                CreateBills(context);
            }
        }

        /// <summary>
        /// Mthod that sets the users as bad payment users when the payment date expires
        /// </summary>
        /// <param name="context">Db Context</param>
        private static void SetBadPaymentHistory(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlRaw("EXEC SetBadPaymentUser");
        }

        /// <summary>
        /// Method that executes the stored procedure to generate the monthly bills
        /// </summary>
        /// <param name="context">Db Context</param>
        private static void CreateBills(ApplicationDbContext context)
        {
            var today = DateTime.Today;
            var comparationDate = today.AddMonths(-1);
            var monthlyBillsAlreadyGenerated = context.GeneratedBill.Any(
                b => b.Year == comparationDate.Year && b.Month == comparationDate.Month
            );

            if (!monthlyBillsAlreadyGenerated)
            {
                var beginDate = new DateTime(comparationDate.Year, comparationDate.Month, 1);
                var endDate = beginDate.AddMonths(1);
                context.Database.ExecuteSqlInterpolated($"EXEC BillCreation {beginDate.ToString("yyyy-MM-dd")}, {endDate.ToString("yyyy-MM-dd")}");
            }
        }
    }
}

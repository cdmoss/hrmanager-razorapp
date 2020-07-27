using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Security;
using System.Timers;

namespace MHFoodBank.TimeClock.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private const string punchClockUrl = "http://10.0.2.2:5000/timeclock/punch-clock";
        private const string getPositionsUrl = "http://10.0.2.2:5000/positions/all";
        private readonly HttpClient _client;
        private readonly Timer _resultTimer;

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _result;
        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        private Position _position;
        public Position Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public DelegateCommand ClockCommand { get; set; }

        public NotifyTaskCompletion<List<Position>> Positions { get; set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };

            _client = new HttpClient(clientHandler);
            _resultTimer = new Timer() { Interval = 5000 };
            _resultTimer.Elapsed += _resultTimer_Elapsed;
            
            ClockCommand = new DelegateCommand(PunchClock);
            Positions = new NotifyTaskCompletion<List<Position>>(GetPositions());
            Result = "Welcome!";
        }

        private void _resultTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Result = "Welcome!";
            _resultTimer.Stop();
        }

        private async void PunchClock()
        {
            ClockedTimeCreateDto dto = new ClockedTimeCreateDto
            {
                Email = _email,
                Password = _password,
                Position = _position.Id
            };

            var json = JsonConvert.SerializeObject(dto);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(punchClockUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PunchClockResult>(responseContent);
                Result = result.Message;
                Email = "";
                Password = "";
                Position = null;
            }

            _resultTimer.Start();
        }

        private async Task<List<Position>> GetPositions()
        {
            try
            {
                var response = await _client.GetAsync(getPositionsUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Position>>(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return null;
        }
    }
}

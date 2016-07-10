using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Sky72C
{
    public class sky72
    {
        private const string sky72Page = "http://www.sky72.com/kr/reservation/real_step01_cal_ttime.jsp"; //date=20090819&course=A&flagcd2=7&holecd=2&resTabno=1";

        public sky72(DateTime p_requireDay, int p_fromTime, int p_tillTime, int p_interval, string p_cellPhones)
        {
            requireDay = p_requireDay;
            fromTime = p_fromTime;
            tillTime = p_tillTime;

            saveMessage = String.Empty;
            interval = p_interval;

            cellPhones = p_cellPhones.Split(';');

            lastSent = DateTime.Now.Subtract(new TimeSpan(0, 0, p_interval));
        }

        private sms m_messager = null;
        private sms Messager
        {
            get
            {
                if (this.m_messager == null)
                    this.m_messager = new sms();
                return this.m_messager;
            }
        }

        private mail m_mailer = null;
        private mail Mailer
        {
            get
            {
                if (this.m_mailer == null)
                    this.m_mailer = new mail();
                return this.m_mailer;
            }
        }

        public int fromTime
        {
            get;
            set;
        }

        private int tillTime
        {
            get;
            set;
        }

        private Random m_random = null;

        private int m_interval = -1;
        private int interval
        {
            get
            {
                if (m_random == null)
                    m_random = new Random(Environment.TickCount);

                return m_random.Next(this.m_interval);
            }
            set
            {
                m_interval = value;
            }
        }

        private string[] cellPhones
        {
            get;
            set;
        }

        private string saveMessage
        {
            get;
            set;
        }

        private DateTime requireDay
        {
            get;
            set;
        }

        private DateTime lastSent
        {
            get;
            set;
        }

        public void StartService()
        {
            string _date = requireDay.ToString("yyyyMMdd");

            while (true)
            {
                int _interval = this.interval;
                Console.WriteLine(String.Format("rescan:{0}, interval:{1} seconds ", DateTime.Now.ToLongTimeString(), _interval));

                if (requireDay.CompareTo(DateTime.Now) < 0)
                {
                    Console.WriteLine(String.Format("i'm realized that time is over, so stop my job. {0} -> {1}", requireDay.ToShortDateString(), DateTime.Now.ToShortDateString()));
                    break;
                }

                string _message = String.Empty;

                _message += this.ExtractTime(_date, "A", 7, 2, 1);
                _message += this.ExtractTime(_date, "B", 7, 2, 1);
                _message += this.ExtractTime(_date, "C", 7, 2, 1);
                _message += this.ExtractTime(_date, "D", 7, 2, 1);

                if (_message.Equals(this.saveMessage) == false)
                {
                    if (String.IsNullOrEmpty(_message) == false)
                    {
                        this.Mailer.SendMailServer("FROM SKY72 RESERVATION SYSTEM", _message, "lisa@oraion.co.kr", "LISA");

                        foreach(string _phone in cellPhones)
                            this.Messager.SimpleMsgService("0", "LISA", "172.20.0.20", _phone, "011-9925-0593", _message + " 예약가능", "SKY72");

                        Console.WriteLine("sent message: " + _message);
                        this.lastSent = DateTime.Now;
                    }

                    this.saveMessage = _message;
                }

                if (Console.KeyAvailable == true)
                {
                    ConsoleKeyInfo _ck = Console.ReadKey();
                    if (_ck.Key == ConsoleKey.Escape)
                        break;
                }

                Thread.Sleep(new TimeSpan(0, 0, _interval));
            }

        }

        public string ExtractTime(string p_date, string p_course, int p_falgcd, int p_holecd, int p_tabno)
        {
            string _result = String.Empty;

            string _url = String.Format("{0}?date={1}&course={2}&flagcd2={3}&holecd={4}&resTabno={5}", sky72.sky72Page, p_date, p_course, p_falgcd, p_holecd, p_tabno);
            
            string _page = this.GetPageAsString(_url);
            string _delimeter = "시";

            string[] _time = _page.Split(_delimeter.ToCharArray());

            foreach (var _s in _time)
            {
                var _lastndx = _s.LastIndexOf(">");
                if (_lastndx > 0 && _lastndx + 1 < _s.Length)
                {
                    var _x = _s.Substring(_lastndx + 1).TrimEnd();

                    if (String.IsNullOrEmpty(_x) == false)
                    {
                        var _t = Convert.ToInt32(_x);
                        if (_t > this.fromTime && _t < this.tillTime)
                        {
                            _result += p_date + "일 " + _t + "시 " + p_course + "코스 ";
                        }
                    }
                }
            }

            return _result;
        }

        public string GetPageAsString(string requestUri)
        {
            string _result = String.Empty;

            try
            {
                // Create the web request   
                HttpWebRequest _request = WebRequest.Create(requestUri) as HttpWebRequest;

                // Get response   
                using (HttpWebResponse _response = _request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream   
                    StreamReader _reader = new StreamReader(_response.GetResponseStream(), Encoding.Default);

                    // Read the whole contents and return as a string   
                    _result = _reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _result;
        }
    }
}

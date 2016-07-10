using System;

namespace Sky72C
{
    public class sms
    {
        private string m_serverIpAddress = null;
        private string ServerIpAddress
        {
            get
            {
                if (this.m_serverIpAddress == null)
                    this.m_serverIpAddress = "211.43.202.33";
                return this.m_serverIpAddress;
            }
        }

        private string m_serverTcpPort = null;
        private string TcpPort
        {
            get
            {
                if (this.m_serverTcpPort == null)
                    this.m_serverTcpPort = "20900";
                return this.m_serverTcpPort;
            }
        }

        private string m_smsCode = null;
        private string SmsCode
        {
            get
            {
                if (this.m_smsCode == null)
                    this.m_smsCode = "CP775775OR";
                return this.m_smsCode;
            }
        }

        private string m_jobCode = null;
        private string JobCode
        {
            get
            {
                if (this.m_jobCode == null)
                    this.m_jobCode = "0";
                return this.m_jobCode;
            }
        }

        private simpleMsgClass m_messageClass = null;
        private simpleMsgClass MessageClass
        {
            get
            {
                if (this.m_messageClass == null)
                    this.m_messageClass = new simpleMsgClass();
                return this.m_messageClass;
            }
        }

        //*********************************************************************************************************//
        // External functions
        //*********************************************************************************************************//
        public string[] SendSMS(params object[] p_params)
        {
            /*
             * p_params
             * 0: seqno
             * 1: sender
             * 2: user's ip-address
             * 3: destination address
             * 4: callback address
             * 5: message
             * 6: request module name
            */

            string _seqno = p_params[0].ToString();			        // 일련번호
            string _sender = p_params[1].ToString();			    // 발송자명
            string _userip = p_params[2].ToString();			    // 발송자의 IP 주소
            string[] _destination = (string[])p_params[3];			// 발신번호
            string _callback = p_params[4].ToString();			    // 회신번호
            string _message = p_params[5].ToString();			    // 메시지
            string _module = p_params[6].ToString();			    // SMS발송을 요청한 모듈명

            string _sqlstr = String.Empty;

            // 마지막에 ';'이 들어간 경우 발신번호, 회신번호가 없으면 발송내역이 없는 것으로 간주
            int _nsize = _destination.Length;
            if (String.IsNullOrEmpty(_destination[_nsize - 1]) == true)
                _nsize--;

            DateTime _now = DateTime.Now;
            string _sid = Guid.NewGuid().ToString().ToUpper();

            //DatParameters _parms = new DatParameters();
            //{
            //    _sqlstr
            //        = "INSERT INTO TB_iSMS_HISTORY00 "
            //        + "( "
            //        + "		sid, callback, message, companyid, sender, module, ipadrs, slmd, sfid "
            //        + ") \n"
            //        + "VALUES "
            //        + "( "
            //        + "		@sid, @callback, @message, @companyid, @sender, @module, @ipadrs, @slmd, @sfid "
            //        + ")";

            //    _parms.Clear();
            //    _parms.Add("@sid", SqlDbType.NVarChar, _sid);
            //    _parms.Add("@callback", SqlDbType.NVarChar, _callback);
            //    _parms.Add("@message", SqlDbType.NVarChar, _message);
            //    _parms.Add("@companyid", SqlDbType.NVarChar, this.CompanyID);
            //    _parms.Add("@sender", SqlDbType.NVarChar, _sender);
            //    _parms.Add("@module", SqlDbType.NVarChar, _module);
            //    _parms.Add("@ipadrs", SqlDbType.NVarChar, _userip);
            //    _parms.Add("@slmd", SqlDbType.DateTime, _now);
            //    _parms.Add("@sfid", SqlDbType.DateTime, _now);

            //    this.g_datHelper.ExecuteText(this.g_smsCProxy.GetConnString, _sqlstr, _parms);
            //}

            string[] _result = new string[_nsize];
            for (int i = 0; i < _nsize; i++)
            {
                // 송,수신 번호 및 전문길이 검사
                int _error = ValidateSMSInfo(_destination[i], _callback, _message);
                if (_error == 0)
                {
                    string _destadr = _destination[i].Replace("-", "").Trim();

                    _now = DateTime.Now;
                    _error = MessageClass.setNetwork(ServerIpAddress, Convert.ToInt32(this.TcpPort), SmsCode);
                    if (_error == 0)
                    {
                        _error = MessageClass.MsgTransSingleJobCode(_seqno, Convert.ToInt32(this.JobCode), _destadr, _callback, _message);

                        //_sqlstr
                        //        = "INSERT INTO TB_iSMS_HISTORY10 "
                        //        + "( "
                        //        + "		sid, uid, seqno, destination, sendtime, error, slmd, sfid "
                        //        + ") \n"
                        //        + "VALUES "
                        //        + "( "
                        //        + "		@sid, @uid, @seqno, @destination, @sendtime, @error, @slmd, @sfid "
                        //        + ")";

                        //_parms.Clear();
                        //_parms.Add("@sid", SqlDbType.NVarChar, _sid);
                        //_parms.Add("@uid", SqlDbType.Decimal, i);
                        //_parms.Add("@seqno", SqlDbType.NVarChar, _seqno);
                        //_parms.Add("@destination", SqlDbType.NVarChar, _destadr);
                        //_parms.Add("@sendtime", SqlDbType.DateTime, _now);
                        //_parms.Add("@error", SqlDbType.Decimal, _error);
                        //_parms.Add("@slmd", SqlDbType.DateTime, _now);
                        //_parms.Add("@sfid", SqlDbType.DateTime, _now);

                        //this.g_datHelper.ExecuteText(this.g_smsCProxy.GetConnString, _sqlstr, _parms);
                    }
                }

                _result[i] = this.GetSMSErrorCode(_error);
            }

            return _result;
        }

        public string SimpleMsgService(string p_seqno, string p_sender, string p_userip, string p_destination, string p_callback, string p_message, string p_module)
        {
            string[] _destination = new string[] { p_destination.Replace("-", "").Trim() };	// 발신번호
            string _callback = p_callback.Replace("-", "").Trim();

            string[] _result = this.SendSMS(p_seqno, p_sender, p_userip, _destination, _callback, p_message, p_module);
            return _result[0];
        }

        //*********************************************************************************************************//
        // Internal functions
        //*********************************************************************************************************//
        private int ValidateSMSInfo(params object[] p_params)
        {
            int _result = 0;

            if (p_params[0].ToString().Replace("-", "").Trim().Length > 12 || p_params[0].ToString() == String.Empty)
                _result = 99;
            else if (p_params[1].ToString().Replace("-", "").Trim().Length > 12 || p_params[1].ToString() == String.Empty)
                _result = 98;
            else if (p_params[2].ToString().Trim().Length > 80 || p_params[2].ToString() == String.Empty)
                _result = 94;

            return _result;
        }

        private string GetSMSErrorCode(int p_error)
        {
            string _result = String.Empty;
            switch (p_error)
            {
                case 0:
                    _result = "전송 성공";
                    break;
                case 99:
                    _result = "잘못된 전송폰 번호";
                    break;
                case 98:
                    _result = "잘못된 회신번호";
                    break;
                case 97:
                    _result = "잘못된 PartnerCode";
                    break;
                case 96:
                    _result = "해지 업체";
                    break;
                case 95:
                    _result = "IP block";
                    break;
                case 94:
                    _result = "잘못된 전문 크기";
                    break;
                case 93:
                    _result = "규정시간 초과 (10sec)";
                    break;
                case 92:
                    _result = "잘못된 업체 정보";
                    break;
                case 91:
                    _result = "해당 job_code 없음";
                    break;
                case 80:
                    _result = "소켓 핸들 생성 실패";
                    break;
                case 81:
                    _result = "소켓 connect 실패";
                    break;
                case 82:
                    _result = "소켓 send 실패";
                    break;
                case 83:
                    _result = "소켓 recv 실패";
                    break;
                default:
                    _result = p_error.ToString();
                    break;
            }

            return _result;
        }

        //*********************************************************************************************************//
        // 
        //*********************************************************************************************************//
    }
}

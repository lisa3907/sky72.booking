using System.Diagnostics;
using Quiksoft.EasyMail.SMTP;

namespace Sky72C
{
    public class mail
    {
        public mail()
        {
            string _lickey = "OraionSoft Inc  (Single Developer)/8202290F138938808F2E5117716068";
            Quiksoft.EasyMail.SMTP.License.Key = _lickey;
        }

        public void SendMailServer(string p_subject, string p_msgbody, string p_email, string p_name)
        {
            try
            {
                //Create the EmailMessage object
                EmailMessage _msgobj = new EmailMessage();
                _msgobj.Subject = p_subject;

                //Add a normal recipient
                _msgobj.Recipients.Add(p_email, p_name, RecipientType.To);

                //Specify the sender
                _msgobj.From.Email = "wego@oraion.co.kr";
                _msgobj.From.Name = "wego tour";

                //Set message body	
                _msgobj.BodyParts.Add(p_msgbody);

                //Specify the mail server and enable authentication
                SMTPServer _server = new SMTPServer();
                _server.Name = "mail.oraion.co.kr";
                _server.Account = "wego";
                _server.Password = "abcde12#";
                _server.AuthMode = SMTPAuthMode.AuthLogin;

                SMTP _smtpobj = new SMTP();
                _smtpobj.SMTPServers.Add(_server);

                //Send the message
                _smtpobj.Send(_msgobj);

                Debug.WriteLine("Message sent.");
            }
            catch (LicenseException LicenseExcep)
            {
                Debug.WriteLine("License key error: " + LicenseExcep.Message);
            }
            catch (FileIOException FileIOExcep)
            {
                Debug.WriteLine("File IO error: " + FileIOExcep.Message);
            }
            catch (SMTPAuthenticationException SMTPAuthExcep)
            {
                Debug.WriteLine("SMTP Authentication error: " + SMTPAuthExcep.Message);
            }
            catch (SMTPConnectionException SMTPConnectExcep)
            {
                Debug.WriteLine("Connection error: " + SMTPConnectExcep.Message);
            }
            catch (SMTPProtocolException SMTPProtocolExcep)
            {
                Debug.WriteLine("SMTP protocol error: " + SMTPProtocolExcep.Message);
            }
        }
    }
}
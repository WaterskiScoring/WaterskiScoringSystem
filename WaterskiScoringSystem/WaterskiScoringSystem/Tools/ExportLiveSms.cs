using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
	public class ExportLiveSms {
		private static String twilioAccountSid = Properties.Settings.Default.TwilioAccount;
		private static String twilioAccountToken = Properties.Settings.Default.TwilioAccountToken;
		private static String twilioFromPhoneNumber = Properties.Settings.Default.TwilioFromPhoneNumber;

		public static bool sendSms(String sendToPhoneNumber, String msg) {
			/*
			 * String inUrl, String inAuthHeaderParms, String inContentType, String inMessage
			 * 
			curl -X POST https://api.twilio.com/2010-04-01/Accounts/{AccountSid}/Messages.json \
			--data-urlencode "From=+15017122661" \
			--data-urlencode "Body=body" \
			--data-urlencode "To=+15558675310" \
			-u AC647a684012fdd63bed159766ddc7b324:your_auth_token
			 */
			String curContentType = "application/json";
			String curSmsLocation = String.Format("https://api.twilio.com/2010-04-01/Accounts/{0}/Messages.json", twilioAccountSid);

			StringBuilder curPostBody = new StringBuilder("");
			curPostBody.Append(Uri.EscapeDataString("From=\"" + twilioFromPhoneNumber + "\""));
			curPostBody.Append(Uri.EscapeDataString(", to=\"" + sendToPhoneNumber + "\""));
			curPostBody.Append(Uri.EscapeDataString(", Body=\"" + msg + "\""));

			//return SendMessageHttp.sendMessagePostAsync(curSmsLocation, curPostBody.ToString(), curContentType, twilioAccountSid, twilioAccountToken );
			// List<KeyValuePair<String, String>> sendMessagePostJsonResp(String inUrl, String inAuthHeaderParms, String inContentType, String inMessage)
			List<KeyValuePair<String, String>> returnResp = SendMessageHttp.sendMessagePostJsonResp(curSmsLocation, null, curContentType, curPostBody.ToString(), twilioAccountSid, twilioAccountToken);
			return true;
		}
	}
}

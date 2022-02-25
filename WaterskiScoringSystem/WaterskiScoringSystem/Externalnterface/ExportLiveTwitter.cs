using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
    class ExportLiveTwitter {
        private int myTourRounds;
        private static String myDefaultConsumerKey = "82KGGTUQ2pIydfkLiHg9g";
        private static String myDefaultConsumerSecret = "4RnSerTyGkE4B6mwzEtfMTKEtK9l65GPyoG5ffL7M";
        private static String myDefaultAccessToken = "1352095230-u79SqoZdbDQ9Q0gOlMFZTUUDRbVsv7PEHEfLECr";
        private static String myDefaultAccessTokenSecret = "g4Cu2sP6T6TQBW8S1AZsTZfNV44ujjgCq6Hu42KuI";
        private static String myDefaultSignatureMethod = "HMAC-SHA1";

        public static String TwitterLocation = "";
        public static String TwitterDefaultAccount = "WaterSkiScoring";
        public static String TwitterAccessPin = "";
        public static String TwitterRequestTokenURL = "";
        public static String TwitterRequestTokenSaveURL = "";
        public static String TwitterRequestToken = "";
        public static String TwitterAccessToken = "";
        public static String TwitterAccessTokenSecret = "";
        public static String TwitterAccessUserid = "";
        public static String TwitterAccessUserScreenName = "";
        public static String TwitterReportByValue = "";

        public ExportLiveTwitter() {
        }

        public static Boolean sendMessage(String inMessage) {
            String curMethodName = "ExportLiveTwitter:sendMessage";
            Boolean returnStatus = true;
            String curTwitterLocation = "https://api.twitter.com/1.1/statuses/update.json";
            List<KeyValuePair<String, String>> curResponseDataList = null;

            String curMessage = inMessage;
            String curMessageParm = "status=" + Uri.EscapeDataString( curMessage );

            String curContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            String curSignatureMethod = myDefaultSignatureMethod;
            String curConsumerKey = myDefaultConsumerKey;
            String curConsumerSecret = myDefaultConsumerSecret;
            String curAccessToken = "", curAccessTokenSecret = "";
            if (ExportLiveTwitter.TwitterRequestTokenURL.Equals( ExportLiveTwitter.TwitterDefaultAccount )) {
                curAccessToken = myDefaultAccessToken;
                curAccessTokenSecret = myDefaultAccessTokenSecret;
            } else {
                curAccessToken = ExportLiveTwitter.TwitterAccessToken;
                curAccessTokenSecret = ExportLiveTwitter.TwitterAccessTokenSecret;
            }

            String curOAuthNonce = Convert.ToBase64String( new ASCIIEncoding().GetBytes( DateTime.Now.Ticks.ToString() ) );
            String curOAuthVersion = Uri.EscapeDataString( "1.0" );
            TimeSpan curTimeSpan = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            String curAuthTimestamp = Convert.ToInt64( curTimeSpan.TotalSeconds ).ToString();

            //Build the signature string
            StringBuilder curBaseString = new StringBuilder( "" );
            curBaseString.Append( "POST" + "&" + Uri.EscapeDataString( curTwitterLocation ) + "&" );
            curBaseString.Append( Uri.EscapeDataString( "oauth_consumer_key=" + curConsumerKey ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_nonce=" + curOAuthNonce ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_signature_method=" + curSignatureMethod ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_timestamp=" + curAuthTimestamp ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_token=" + curAccessToken ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_version=" + curOAuthVersion ) );
            curBaseString.Append( Uri.EscapeDataString( "&status=" + Uri.EscapeDataString(curMessage) ) );

            // Sign the request
            String curSigningKey = Uri.EscapeDataString( curConsumerSecret ) + "&" + Uri.EscapeDataString( curAccessTokenSecret );
            HMACSHA1 curHasher = new HMACSHA1( new ASCIIEncoding().GetBytes( curSigningKey ) );
            String curSignature = Convert.ToBase64String( curHasher.ComputeHash( new ASCIIEncoding().GetBytes( curBaseString.ToString() ) ) );
            ServicePointManager.Expect100Continue = false;

            StringBuilder curAuthHeaderParm = new StringBuilder( "" );
            curAuthHeaderParm.Append( "OAuth " );
            curAuthHeaderParm.Append( "oauth_nonce=\"" + Uri.EscapeDataString( curOAuthNonce ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_signature_method=\"" + Uri.EscapeDataString( curSignatureMethod ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_timestamp=\"" + Uri.EscapeDataString( curAuthTimestamp ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_consumer_key=\"" + Uri.EscapeDataString( curConsumerKey ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_token=\"" + Uri.EscapeDataString( curAccessToken ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_signature=\"" + Uri.EscapeDataString( curSignature ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_version=\"" + curOAuthVersion + "\"" );

            try {
                returnStatus = SendMessageHttp.sendMessagePostWithHeader( curTwitterLocation, curAuthHeaderParm.ToString(), curContentType, curMessageParm );
            } catch (Exception ex) {
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return returnStatus;
        }

        public static Boolean getAppAccessAuthorize() {
            String curMethodName = "ExportLiveTwitter:getAppAccessAuthorize";
            Boolean returnStatus = true;
            String curTwitterLocation = "https://api.twitter.com/oauth/authorize";

            if (ExportLiveTwitter.TwitterAccessPin.Length > 1) {
                if (ExportLiveTwitter.TwitterAccessToken.Length > 1 && ExportLiveTwitter.TwitterAccessTokenSecret.Length > 1) {
                } else {
                    getAppAccessToken();
                }
            } else {
                getAppRequestToken();
                String curMessageAuthParm = "?oauth_token=" + ExportLiveTwitter.TwitterRequestToken;

                try {
                    ExportLiveTwitter.TwitterRequestTokenURL = curTwitterLocation + curMessageAuthParm;
                } catch (Exception ex) {
                    returnStatus = false;
                    MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
            }

            return returnStatus;
        }

        public static void getAppRequestToken() {
            String curMethodName = "ExportLiveTwitter:getAppRequestToken";
            String curTwitterLocation = "https://api.twitter.com/oauth/request_token";
            List<KeyValuePair<String, String>> curResponseDataList = null;

            String curContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            //String curMessageAuthParm = "grant_type=client_credentials";
            String curMessageAuthParm = "";
            String curSignatureMethod = myDefaultSignatureMethod;
            String curConsumerKey = myDefaultConsumerKey ;
            String curConsumerSecret = myDefaultConsumerSecret;
            String curAccessTokenSecret = "";

            String curOAuthNonce = Convert.ToBase64String( new ASCIIEncoding().GetBytes( DateTime.Now.Ticks.ToString() ) );
            String curOAuthVersion = Uri.EscapeDataString("1.0");
            TimeSpan curTimeSpan = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            String curAuthTimestamp = Convert.ToInt64( curTimeSpan.TotalSeconds ).ToString();

            //Build the signature string
            StringBuilder curBaseString = new StringBuilder( "" );
            curBaseString.Append( "POST" + "&" + Uri.EscapeDataString( curTwitterLocation ) + "&" );
            curBaseString.Append( Uri.EscapeDataString( "oauth_callback=oob" ));
            curBaseString.Append( Uri.EscapeDataString( "&oauth_consumer_key=" + curConsumerKey ));
            curBaseString.Append( Uri.EscapeDataString( "&oauth_nonce=" + curOAuthNonce ));
            curBaseString.Append( Uri.EscapeDataString( "&oauth_signature_method=" + curSignatureMethod ));
            curBaseString.Append( Uri.EscapeDataString( "&oauth_timestamp=" + curAuthTimestamp ));
            curBaseString.Append( Uri.EscapeDataString( "&oauth_version=" + curOAuthVersion ));

            // Sign the request
            String curSigningKey = Uri.EscapeDataString( curConsumerSecret ) + "&" + Uri.EscapeDataString( curAccessTokenSecret );
            HMACSHA1 curHasher = new HMACSHA1( new ASCIIEncoding().GetBytes( curSigningKey ) );
            String curSignature = Convert.ToBase64String( curHasher.ComputeHash( new ASCIIEncoding().GetBytes( curBaseString.ToString() ) ) );

            StringBuilder curAuthHeaderParm = new StringBuilder( "" );
            curAuthHeaderParm.Append( "OAuth oauth_callback=\"oob\"" );
            curAuthHeaderParm.Append( ", oauth_consumer_key=\"" + Uri.EscapeDataString( curConsumerKey ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_nonce=\"" + Uri.EscapeDataString( curOAuthNonce ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_signature_method=\"" + Uri.EscapeDataString(curSignatureMethod) + "\"" );
            curAuthHeaderParm.Append( ", oauth_signature=\"" + Uri.EscapeDataString( curSignature ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_timestamp=\"" + Uri.EscapeDataString( curAuthTimestamp ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_version=\"" + curOAuthVersion + "\"" );

            try {
                curResponseDataList = SendMessageHttp.sendMessagePostJsonResp( curTwitterLocation, curAuthHeaderParm.ToString(), curContentType, curMessageAuthParm );
                if (curResponseDataList.Count > 0) {
                    foreach (KeyValuePair<String, String> curEntry in curResponseDataList) {
                        if (curEntry.Key.Equals( "oauth_token" )) {
                            ExportLiveTwitter.TwitterRequestToken = curEntry.Value;
                        } else if (curEntry.Key.Equals( "oauth_token_secret" )) {
                            ExportLiveTwitter.TwitterAccessTokenSecret = curEntry.Value;
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                ExportLiveTwitter.TwitterRequestToken = "";
                ExportLiveTwitter.TwitterAccessTokenSecret = "";
            }
        }

        public static Boolean getAppAccessToken() {
            String curMethodName = "ExportLiveTwitter:getAppAccessToken";
            Boolean returnStatus = true;
            String curTwitterLocation = "https://api.twitter.com/oauth/access_token";
            List<KeyValuePair<String, String>> curResponseDataList = null;

            //String curContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            String curContentType = "application/json";
            String curSignatureMethod = myDefaultSignatureMethod;
            String curConsumerKey = myDefaultConsumerKey;
            String curConsumerSecret = myDefaultConsumerSecret;

            String curOAuthNonce = Convert.ToBase64String( new ASCIIEncoding().GetBytes( DateTime.Now.Ticks.ToString() ) );
            String curOAuthVersion = Uri.EscapeDataString( "1.0" );
            TimeSpan curTimeSpan = DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            String curAuthTimestamp = Convert.ToInt64( curTimeSpan.TotalSeconds ).ToString();

            //Build the signature string
            StringBuilder curBaseString = new StringBuilder( "" );
            curBaseString.Append( "POST" + "&" + Uri.EscapeDataString( curTwitterLocation ) + "&" );
            curBaseString.Append( Uri.EscapeDataString( "oauth_consumer_key=" + curConsumerKey ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_nonce=" + curOAuthNonce ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_signature_method=" + curSignatureMethod ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_timestamp=" + curAuthTimestamp ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_token=" + ExportLiveTwitter.TwitterRequestToken ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_verifier=" + ExportLiveTwitter.TwitterAccessPin ) );
            curBaseString.Append( Uri.EscapeDataString( "&oauth_version=" + curOAuthVersion ) );

            // Sign the request
            String curSigningKey = Uri.EscapeDataString( curConsumerSecret ) + "&" + Uri.EscapeDataString( ExportLiveTwitter.TwitterAccessTokenSecret );
            HMACSHA1 curHasher = new HMACSHA1( new ASCIIEncoding().GetBytes( curSigningKey ) );
            String curSignature = Convert.ToBase64String( curHasher.ComputeHash( new ASCIIEncoding().GetBytes( curBaseString.ToString() ) ) );

            StringBuilder curAuthHeaderParm = new StringBuilder( "" );
            curAuthHeaderParm.Append( "OAuth auth_consumer_key=\"" + Uri.EscapeDataString( curConsumerKey ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_nonce=\"" + Uri.EscapeDataString( curOAuthNonce ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_signature_method=\"" + Uri.EscapeDataString( curSignatureMethod ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_signature=\"" + Uri.EscapeDataString( curSignature ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_timestamp=\"" + Uri.EscapeDataString( curAuthTimestamp ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_token=\"" + Uri.EscapeDataString( ExportLiveTwitter.TwitterRequestToken ) + "\"" );
            curAuthHeaderParm.Append( ", oauth_verifier=\"" + Uri.EscapeDataString(ExportLiveTwitter.TwitterAccessPin + "\"" ) );
            curAuthHeaderParm.Append( ", oauth_version=\"" + curOAuthVersion + "\"" );

            String curMessageAuthParm = "oauth_verifier=" + ExportLiveTwitter.TwitterAccessPin;

            try {
                curResponseDataList = SendMessageHttp.sendMessagePostJsonResp( curTwitterLocation, curAuthHeaderParm.ToString(), curContentType, curMessageAuthParm );
                if (curResponseDataList.Count > 0) {
                    foreach (KeyValuePair<String, String> curEntry in curResponseDataList) {
                        if (curEntry.Key.Equals( "oauth_token" )) {
                            ExportLiveTwitter.TwitterAccessToken = curEntry.Value;
                        } else if (curEntry.Key.Equals( "oauth_token_secret" )) {
                            ExportLiveTwitter.TwitterAccessTokenSecret = curEntry.Value;
                        } else if (curEntry.Key.Equals( "user_id" )) {
                            ExportLiveTwitter.TwitterAccessUserid = curEntry.Value;
                        } else if (curEntry.Key.Equals( "screen_name" )) {
                            ExportLiveTwitter.TwitterAccessUserScreenName = curEntry.Value;
                        }
                    }
                }
            } catch (Exception ex) {
                returnStatus = false;
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                ExportLiveTwitter.TwitterAccessToken = "";
                ExportLiveTwitter.TwitterAccessTokenSecret = "";
                ExportLiveTwitter.TwitterAccessUserid = "";
                ExportLiveTwitter.TwitterAccessUserScreenName = "";
            }

            return returnStatus;
        }

    }
}

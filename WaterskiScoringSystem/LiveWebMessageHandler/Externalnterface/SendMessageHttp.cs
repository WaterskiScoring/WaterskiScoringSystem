using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

using LiveWebMessageHandler.Common;
using System.Threading;

namespace LiveWebMessageHandler.Externalnterface {
    class SendMessageHttp {
        private static ManualResetEvent myManualResetEvent = new ManualResetEvent( false );

        public SendMessageHttp() {
        }

        public static Dictionary<string, object> sendMessagePostFileUpload(String inUrl, String inFileRef, String inFileFormName, NameValueCollection inHeaderParams, NameValueCollection inFormData, String inUserAccount, String inPassword) {
            String curMethodName = "SendMessageHttp:sendMessagePostFileUpload: ";
            HttpWebRequest curRequest = null;
            String curFileFormName = "file";
            StringBuilder curMessageBuffer = new StringBuilder( "" );

            try {
                String curBoundary = "----------" + DateTime.Now.Ticks.ToString( "x" );
                String curBoundaryLine = "\r\n--" + curBoundary + "\r\n";
                String curBoundaryTrailer = "\r\n--" + curBoundary + "--\r\n";

                if (( inFileFormName != null ) && ( inFileFormName.Length > 0 )) {
                    curFileFormName = inFileFormName;
                }

                //Create a request using a URL that can receive a post
                curRequest = (HttpWebRequest)WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;

				//Set the Method property of the request to POST.
				curRequest.Method = "POST";
                curRequest.KeepAlive = true;
                curRequest.Timeout = 500000;

				if ( inUserAccount != null ) {
					if ( inUrl.Contains( "usawaterski" ) ) {
						inHeaderParams.Add( "WSTIMS", "Basic " + inUserAccount + ":" + inPassword );
					} else if ( inUrl.Contains( "waterskiresults" ) ) {
						curRequest.Headers["WSTIMSAPI"] = "LiveWebScoreboard";
					} else {
						curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
					}
				} else if ( inUrl.Contains( "waterskiresults" ) ) {
					curRequest.Headers["WSTIMSAPI"] = "LiveWebScoreboard";
				}
				if (inUrl.ToLower().StartsWith( "https" )) {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback( AcceptAllCertifications );
                    ServicePointManager.Expect100Continue = false;
                }

                //Set the ContentType property of the WebRequest.
                curRequest.ContentType = "multipart/form-data; boundary=" + curBoundary;

                //Set header parameters to the WebRequest
                ( (HttpWebRequest)curRequest ).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";
                if (inHeaderParams != null) {
                    foreach (string curKey in inHeaderParams.Keys) {
                        curRequest.Headers[curKey] = inHeaderParams[curKey];
                    }
                }

                //Format and write form data to request container stream
                if (inFormData == null) {
                    curMessageBuffer.Append( curBoundaryLine );
                    //curRequestStream.Write( curBoundaryBytes, 0, curBoundaryBytes.Length );
                } else {
                    String curFormDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string curKey in inFormData.Keys) {
                        curMessageBuffer.Append( curBoundaryLine );
                        if (curKey.StartsWith( "tag_names[" )) {
                            curMessageBuffer.Append( String.Format( curFormDataTemplate, "tag_names[]", inFormData[curKey] ) );
                        } else {
                            curMessageBuffer.Append( String.Format( curFormDataTemplate, curKey, inFormData[curKey] ) );
                        }
                    }
                    curMessageBuffer.Append( curBoundaryLine );
                }

                //Format and write form data for uploading file to request container stream
                String curFileContentType = "application/octet-stream";
                String curHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                curMessageBuffer.Append( String.Format( curHeaderTemplate, curFileFormName, Path.GetFileName( inFileRef ), curFileContentType ) );

                // Write out the file contents
                FileStream curFileStream = new FileStream( inFileRef, FileMode.Open, FileAccess.Read );

                curRequest.ContentLength = curMessageBuffer.Length + curFileStream.Length + curBoundaryTrailer.Length;

                //Get stream container for request data
                Stream curRequestStream = curRequest.GetRequestStream();
                curRequestStream.Write( Encoding.UTF8.GetBytes( curMessageBuffer.ToString() ), 0, curMessageBuffer.Length );

                //Write file contents to HTTP request
                Byte[] curFileBuffer = new byte[4096];
                int curBytesRead = 0;
                while (( curBytesRead = curFileStream.Read( curFileBuffer, 0, curFileBuffer.Length ) ) != 0) {
                    curRequestStream.Write( curFileBuffer, 0, curBytesRead );
                }
                curFileStream.Close();

                curRequestStream.Write( Encoding.UTF8.GetBytes( curBoundaryTrailer ), 0, curBoundaryTrailer.Length );
                curRequestStream.Close();

				//Send request to upload file
				Log.WriteFile( String.Format( "{0}Sending request to upload file: {1}", curMethodName, inFileRef ) );

				HttpWebResponse curResp = (HttpWebResponse)curRequest.GetResponse();
				JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
				jsonSerializer.MaxJsonLength = Int32.MaxValue;
				return jsonSerializer.Deserialize<Dictionary<string, object>>( getResponseAsString( curResp ) );

            } catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );
                return new Dictionary<string, object>() { { "Error", "Exception encountered loading video: " + ex.Message } } ;
            }
        }

        public static String sendMessagePostXml(String inUrl, String inMessage) {
            return sendMessagePost( inUrl, inMessage, "text/xml;charset=\"utf-8\"", null, null ); // 415
		}
		
		public static String sendMessagePostJson( String inUrl, String inMessage ) {
			return sendMessagePost( inUrl, inMessage, "application/json;charset=\"utf-8\"", null, null );
		}

		public static String sendMessagePost( String inUrl, String inMessage, String inContentType, String inUserAccount, String inPassword ) {
            String curMethodName = "SendMessageHttp:sendMessagePostAsync: ";
            String curMsg = "";
            WebRequest curRequest = null;
            Stream curDataStream = null;

            try {
                // Create a request using a URL that can receive a post. 
                // Set the Method property of the request to POST.
                curRequest = WebRequest.Create( inUrl );
                if ( curRequest == null ) {
                    curMsg = String.Format( "{0}Unable to connect to {1}", curMethodName, inUrl );
                    throw new Exception( curMsg );
                }

				( (HttpWebRequest)curRequest ).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";
				( (HttpWebRequest)curRequest ).MediaType = "text/xml";

				curRequest.Method = "POST";
                if ( inUserAccount != null ) {
                    if ( inUrl.Contains( "usawaterski" ) ) {
                        curRequest.Headers["WSTIMSAPI"] = "Basic " + inUserAccount + ":" + inPassword;
                    } else if ( inUrl.Contains( "waterskiresults" ) ) {
						curRequest.Headers["WSTIMSAPI"] = "LiveWebScoreboard";
					} else {
                        curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
                    }
				} else if ( inUrl.Contains( "waterskiresults" ) ) {
					curRequest.Headers["WSTIMSAPI"] = "LiveWebScoreboard";
				}

				// Set the ContentType property of the WebRequest.
				if ( inContentType == null ) {
                    curRequest.ContentType = "application/x-www-form-urlencoded";
                } else {
                    curRequest.ContentType = inContentType;
                }

                // Create POST data and convert it to a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes( inMessage );

                // Set the ContentLength property of the WebRequest.
                curRequest.ContentLength = byteArray.Length;

                // Get the request stream.
                curDataStream = curRequest.GetRequestStream();

                // Write the data to the request stream.
                curDataStream.Write( byteArray, 0, byteArray.Length );

                // Close the Stream object.
                curDataStream.Close();

                HttpWebResponse curResponse = (HttpWebResponse)curRequest.GetResponse();
                // Display the status.
                curMsg = ( (HttpWebResponse)curResponse ).StatusDescription;
				if ( curMsg.Equals( "OK" ) || curMsg.Equals( "Created" ) ) {
                    return getResponseAsString( curResponse );
                }

                curMsg = String.Format( "{0}Unexpected response: {1}", curMethodName, inUrl, curMsg );
                throw new Exception( curMsg );

            } catch ( Exception ex ) {
                curMsg = String.Format( "{0} Likely temporary loss of internet connection {1}", curMethodName, ex.Message );
                throw new Exception( curMsg );

            } finally {
                // Clean up the streams.
                if ( curDataStream != null ) curDataStream.Close();
            }
        }

        public static bool sendMessagePostAsync(String inUrl, String inMessage, String inContentType, String inUserAccount, String inPassword) {
            String curMethodName = "SendMessageHttp:sendMessagePostAsync: ";
            WebRequest curRequest = null;
            RequestState curRequestState = null;

            String[] curUrlList = inUrl.Split( '|' );

            try {
                // Create a request using a URL that can receive a post. 
                // Set the Method property of the request to POST.
                foreach (String curUrl in curUrlList) {
                    curRequest = WebRequest.Create( curUrl );
					if ( curRequest == null) {
                        MessageBox.Show( curMethodName + ":Unable to connect to " + curUrl );
                        Log.WriteFile( curMethodName + ":Unable to connect to " + curUrl );

					} else {
                        curRequest.Method = "POST";
						if ( inUserAccount != null ) {
							if ( inUrl.Contains( "usawaterski" ) ) {
								curRequest.Headers["WSTIMS"] = "Basic " + inUserAccount + ":" + inPassword;
							} else {
								curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
							}
						}

                        // Set the ContentType property of the WebRequest.
                        if (inContentType == null) {
                            curRequest.ContentType = "application/x-www-form-urlencoded";
                        } else {
                            curRequest.ContentType = inContentType;
                        }

                        curRequestState = new RequestState();
                        curRequestState.WebReqst = curRequest;
                        curRequestState.InputMsgBuffer = Encoding.UTF8.GetBytes( inMessage );
                        curRequestState.WebReqst.ContentLength = curRequestState.InputMsgBuffer.Length;

                        // Get the request stream.
                        curRequest.BeginGetRequestStream( new AsyncCallback( GetRequestStreamCallback ), curRequestState );
                    }
                }

            } catch (Exception ex) {
				MessageBox.Show( String.Format( "{0} Likely temporary loss of internet connection {1}", curMethodName, ex.Message ) );
				return false;
            }

            return true;
        }

		public static String getResponseAsString( HttpWebResponse curResponse ) {
			String curMethodName = "getResponseAsString: ";
			Stream curResponseStream = null;
			StreamReader curStreamReader = null;
			String curResponseMessage = "";

			try {
				curResponseStream = curResponse.GetResponseStream();
				curStreamReader = new StreamReader( curResponseStream );
				curResponseMessage = curStreamReader.ReadToEnd();
				if ( curResponseMessage.Length > 0 ) {
					return curResponseMessage;
				}
				return null;

			} catch ( Exception ex ) {
				MessageBox.Show( curMethodName + String.Format( "Mesage={0}\n\nException:{1}", curResponseMessage, ex.Message ) );
				Log.WriteFile( curMethodName + String.Format( "Mesage={0}\n\nException:{1}", curResponseMessage, ex.Message ) );
				return null;

			} finally {
				// Clean up the streams.
				if ( curStreamReader != null ) curStreamReader.Close();
				if ( curResponseStream != null ) curResponseStream.Close();
				if ( curResponse != null ) curResponse.Close();
			}
		}

        private static void GetRequestStreamCallback(IAsyncResult inAsyncResult) {
			String curMethodName = "SendMessageHttp:GetRequestStreamCallback: ";
			if ( inAsyncResult == null ) {
				MessageBox.Show( "GetRequestStreamCallback: AsyncResult is empty, most likely a bad internet connection" );
				return;
			}

			RequestState curRequestState = null;
			try {
				inAsyncResult.AsyncWaitHandle.WaitOne();
				curRequestState = (RequestState) inAsyncResult.AsyncState;
				if ( curRequestState.InputMsgBuffer.Length > 0 ) {
					using ( Stream curDataStream = curRequestState.WebReqst.EndGetRequestStream( inAsyncResult ) ) {
						curDataStream.Write( curRequestState.InputMsgBuffer, 0, curRequestState.InputMsgBuffer.Length );
						curDataStream.Close();
					}
				}
				return;

			} catch (Exception ex ) {
				Log.WriteFile( curMethodName + ":Exception:" + ex.Message );
				throw new Exception( ex.Message );

			} finally {
				// Close the wait handle.
				if ( inAsyncResult != null && inAsyncResult.AsyncWaitHandle != null ) {
					inAsyncResult.AsyncWaitHandle.Close();
				}

				// Start the asynchronous operation to get the response
				if ( curRequestState != null && curRequestState.WebReqst != null ) {
					curRequestState.WebReqst.BeginGetResponse( new AsyncCallback( GetRespCallBack ), curRequestState );
				}
			}
		}

        private static void GetRespCallBack(IAsyncResult inAsyncResult) {
            String curMethodName = "SendMessageHttp:GetRespCallBack: ";
			RequestState curRequestState = null;
			try {
				// Set the State of request to asynchronous.
				// End the Asynchronous response.
				if ( inAsyncResult.AsyncState == null ) return;
				curRequestState = (RequestState) inAsyncResult.AsyncState;
				if ( curRequestState == null ) return;
				if ( curRequestState.WebReqst == null ) return;
				curRequestState.WebResp = curRequestState.WebReqst.EndGetResponse( inAsyncResult );
				String curMsg = ( (HttpWebResponse) curRequestState.WebResp ).StatusDescription;
				if ( !( curMsg.Equals( "OK" ) ) ) {
					Log.WriteFile( String.Format( "{0}Unexpected response for HTTP request {1} : {2}", curMethodName, curRequestState.WebReqst.RequestUri.ToString(), curMsg ) );
					MessageBox.Show( String.Format( "{0}Unexpected response for HTTP request: {1} : {2}", curMethodName, curRequestState.WebReqst.RequestUri.ToString(), curMsg ) );
					return;
				}

				// Read the response into a 'Stream' object.
				// Open the stream using a StreamReader for easy access.
				// Read the content.
				curRequestState.WebRespStream = curRequestState.WebResp.GetResponseStream();
				StringBuilder responseFromServer = new StringBuilder( "" );
				using ( StreamReader curReader = new StreamReader( curRequestState.WebRespStream ) ) {
					responseFromServer = new StringBuilder( curReader.ReadToEnd() );
				}
				if ( responseFromServer.Length > 0 ) {
					Log.WriteFile( curMethodName + ":responseFromServer:" + responseFromServer.ToString() );
				}

			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );

			} finally {
				if ( curRequestState != null && curRequestState.WebRespStream != null ) {
					curRequestState.WebRespStream.Close();
					curRequestState.WebResp.Close();
				}
				myManualResetEvent.Set();
			}
		}

        private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            //solve the problem of invalid certificates - accept all as valid
            return true;
        }

	}

	public class RequestState {
        // This class stores the state of the request. 
        public StringBuilder ReqstData;
        public byte[] InputMsgBuffer;
        public WebRequest WebReqst;
        public WebResponse WebResp;
        public Stream WebRespStream;
        public RequestState() {
            InputMsgBuffer = new byte[0];
            ReqstData = new StringBuilder( "" );
            WebResp = null;
            WebRespStream = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
    class SendMessageHttp {
        private static ManualResetEvent myManualResetEvent = new ManualResetEvent( false );

        public SendMessageHttp() {
        }

        public static Dictionary<string, object> getMessageResponseJson(String inUrl, NameValueCollection inHeaderParams, String inContentType) {
            return getMessageResponseJson( inUrl, inHeaderParams, inContentType, null, null, false );
        }
        public static Dictionary<string, object> getMessageResponseJson(String inUrl, NameValueCollection inHeaderParams, String inContentType, String inUserAccount, String inPassword, bool inPostMethod) {
            String curMethodName = "SendMessageHttp:getMessageResponseJson: ";
            HttpWebRequest curRequest = null;

            try {
                //Create a request using a URL that can receive a post
                curRequest = (HttpWebRequest)WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;

				//Set the Method property of the request to POST.
				if (inPostMethod) {
                    curRequest.Method = "POST";
                } else {
                    curRequest.Method = "GET";
                }
                //Set the ContentType property of the WebRequest.
                curRequest.ContentType = inContentType;
                curRequest.KeepAlive = true;
                curRequest.Timeout = 500000;

				if ( inUserAccount != null ) {
					if ( inUrl.Contains( "usawaterski" ) ) {
						inHeaderParams.Add( "WSTIMS", "Basic " + inUserAccount + ":" + inPassword );
					} else {
						curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
					}
				}
				if (inUrl.ToLower().StartsWith( "https" )) {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback( AcceptAllCertifications );
                    ServicePointManager.Expect100Continue = false;
                }

                //Set header parameters to the WebRequest
                ( (HttpWebRequest)curRequest ).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";
                if (inHeaderParams != null) {
                    foreach (string curKey in inHeaderParams.Keys) {
                        curRequest.Headers[curKey] = inHeaderParams[curKey];
                    }
                }

				//Send request to upload file
				HttpWebResponse curResp = (HttpWebResponse)curRequest.GetResponse();
				JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
				jsonSerializer.MaxJsonLength = Int32.MaxValue;
				return jsonSerializer.Deserialize<Dictionary<string, object>>( getResponseAsString( curResp ) );

			} catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );
				return null;
            }
        }
		public static DataTable getMessageResponseDataTable(String inUrl, NameValueCollection inHeaderParams, String inContentType, String inUserAccount, String inPassword, bool inPostMethod) {
			String curMethodName = "SendMessageHttp:getMessageResponseDataTable: ";
			HttpWebRequest curRequest = null;

			try {
				//Create a request using a URL that can receive a post
				curRequest = (HttpWebRequest)WebRequest.Create(inUrl);
				curRequest.AllowAutoRedirect = false;

				//Set the Method property of the request to POST.
				if ( inPostMethod) {
					curRequest.Method = "POST";
				} else {
					curRequest.Method = "GET";
				}
				//Set the ContentType property of the WebRequest.
				curRequest.ContentType = inContentType;
				curRequest.KeepAlive = true;
				curRequest.Timeout = 500000;

				if (inUserAccount != null) {
					if (inUrl.Contains("usawaterski")) {
						inHeaderParams.Add("WSTIMS", "Basic " + inUserAccount + ":" + inPassword);
					} else {
						curRequest.Credentials = new NetworkCredential(inUserAccount, inPassword);
					}
				}
				if (inUrl.ToLower().StartsWith("https")) {
					ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
					ServicePointManager.Expect100Continue = false;
				}

				//Set header parameters to the WebRequest
				((HttpWebRequest)curRequest).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";

				if (inHeaderParams != null) {
					foreach (string curKey in inHeaderParams.Keys) {
						curRequest.Headers[curKey] = inHeaderParams[curKey];
					}
				}

				//Send request to upload file
				return readJsonResponseAsDataTable((HttpWebResponse)curRequest.GetResponse());

			} catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection {1}", curMethodName, ex.Message ) );
				return null;
			}
		}

		public static List<object> getMessageResponseJsonArray( String inUrl, NameValueCollection inHeaderParams, String inContentType, String inUserAccount, String inPassword, bool inPostMethod ) {
            String curMethodName = "SendMessageHttp:getMessageResponseJsonArray: ";
            HttpWebRequest curRequest = null;

            try {
				//Create a request using a URL that can receive a post
				curRequest = (HttpWebRequest) WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;

				//Set the Method property of the request to POST.
				if ( inPostMethod ) {
                    curRequest.Method = "POST";
                } else {
                    curRequest.Method = "GET";
                }
                //Set the ContentType property of the WebRequest.
                curRequest.ContentType = inContentType;
                curRequest.KeepAlive = true;
                curRequest.Timeout = 500000;

				if ( inUserAccount != null ) {
					if ( inUrl.Contains( "usawaterski" ) ) {
						inHeaderParams.Add( "WSTIMS", "Basic " + inUserAccount + ":" + inPassword );
					} else {
						curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
					}
				}
				if ( inUrl.ToLower().StartsWith("https") ) {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                    ServicePointManager.Expect100Continue = false;
                }

				//Set header parameters to the WebRequest
				( (HttpWebRequest) curRequest ).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";

				if ( inHeaderParams != null ) {
                    foreach ( string curKey in inHeaderParams.Keys ) {
                        curRequest.Headers[curKey] = inHeaderParams[curKey];
                    }
                }

				//Send request to upload file
				return readJsonResponseAsList((HttpWebResponse)curRequest.GetResponse() );

            } catch ( Exception ex ) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection {1}", curMethodName, ex.Message ) );
				return null;
            }
        }

        public static bool getDownloadFile( String inUrl, String inSaveFileLocation ) {
            String curMethodName = "SendMessageHttp:getDownloadFile: ";
            WebClient curWebClientRequest = null;

            try {
                curWebClientRequest = new WebClient();
                curWebClientRequest.DownloadFile( inUrl, inSaveFileLocation );
                return true;
            
            } catch ( Exception ex ) {
                String curMsg = String.Format( "{0}Exception encountered {1},  Download File: {2}, Save Location: {3}"
                    , curMethodName, ex.Message, inUrl, inSaveFileLocation );
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                return false;
            }
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
                curRequest.Timeout = 10000;
                if ( inUrl.Contains( "sproutvideo" ) ) curRequest.Timeout = 500000;

				if ( inUserAccount != null ) {
					if ( inUrl.Contains( "usawaterski" ) ) {
						inHeaderParams.Add( "WSTIMS", "Basic " + inUserAccount + ":" + inPassword );
					} else {
						curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
					}
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
        public static List<KeyValuePair<String, String>> sendMessagePostJsonResp(String inUrl, String inAuthHeaderParms, String inContentType, String inMessage) {
            return sendMessagePostJsonResp( inUrl, inAuthHeaderParms, inContentType, inMessage, null, null );
        }

        public static List<KeyValuePair<String, String>> sendMessagePostJsonResp(String inUrl, String inAuthHeaderParms, String inContentType, String inMessage, String inUserAccount, String inPassword) {
            String curMethodName = "SendMessageHttp:sendMessagePostJsonResp: ";
            HttpWebRequest curRequest = null;
            String curRespMessage = "";
            Stream curDataStream = null;
            WebResponse curResponse = null;
            StreamReader curReader = null;
            String curMsg = "";
            List<KeyValuePair<String, String>> curResponseDataList = null;

            try {
                // Create a request using a URL that can receive a post. 
                // Set the Method property of the request to POST.
                curRequest = (HttpWebRequest)WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;
				curRequest.Method = "POST";
                curRequest.KeepAlive = false;
                curRequest.ContentType = inContentType;
                if (inAuthHeaderParms != null) {
                    curRequest.Headers.Add( "Authorization", inAuthHeaderParms );
                }
                if ( inUserAccount != null ) {
                    if (inUrl.Contains("usawaterski")) {
                        curRequest.Headers["WSTIMS"] = "Basic " + inUserAccount + ":" + inPassword;
                    } else {
                        curRequest.Credentials = new NetworkCredential(inUserAccount, inPassword);
                    }
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

                // Get the response.
                curResponse = curRequest.GetResponse();

                // Display the status.
                curMsg = ( (HttpWebResponse)curResponse ).StatusDescription;
                if (curMsg.Equals( "OK" )) {
                } else {
					Log.WriteFile( String.Format( "{0}Unexpected response: {1}", curMethodName, inUrl, curMsg ) );
					MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1} : {2}", curMethodName, inUrl, curMsg ) );
                }

                // Get the stream containing content returned by the server.
                curDataStream = curResponse.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                curReader = new StreamReader( curDataStream );

                // Read the content.
                curRespMessage = curReader.ReadToEnd();
                if (curRespMessage.Length > 0) {
                    curResponseDataList = readJsonResponse( curRespMessage );
                    //showServerRespJson( curResponseDataList );
                }
            
            } catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );
                curResponseDataList = null;
            
			} finally {
                // Clean up the streams.
                if (curReader != null) curReader.Close();
                if (curDataStream != null) curDataStream.Close();
                if (curResponse != null) curResponse.Close();
            }

            return curResponseDataList;
        }

		public static Dictionary<string, object> getMessageDictionaryPostMessage( String inUrl, NameValueCollection inHeaderParams, String inContentType, String inMessage, String inUserAccount, String inPassword  ) {
            String curMethodName = "SendMessageHttp:readMessageDictionaryPostMessage: ";
            HttpWebRequest curRequest = null;
			JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
			jsonSerializer.MaxJsonLength = Int32.MaxValue;
			Stream curDataStream = null;

            try {
                // Create a request using a URL that can receive a post. 
                // Set the Method property of the request to POST.
                curRequest = (HttpWebRequest)WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;
				curRequest.Method = "POST";
                curRequest.KeepAlive = false;
                curRequest.ContentType = inContentType;
				curRequest.Accept = inContentType;

				if ( inUserAccount != null ) {
					if ( inUrl.Contains( "usawaterski" ) ) {
						curRequest.Headers["WSTIMS"] = "Basic " + inUserAccount + ":" + inPassword;
					} else {
						curRequest.Credentials = new NetworkCredential( inUserAccount, inPassword );
					}
				}
				
				//Set header parameters to the WebRequest
				( (HttpWebRequest)curRequest ).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";
                if (inHeaderParams != null) {
                    foreach (string curKey in inHeaderParams.Keys) {
                        curRequest.Headers[curKey] = inHeaderParams[curKey];
                    }
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

				// Get the response.
				HttpWebResponse curResp = ( HttpWebResponse)curRequest.GetResponse();

				return jsonSerializer.Deserialize<Dictionary<string, object>>( getResponseAsString( curResp ) );

			} catch ( WebException e ) {
				using ( WebResponse response = e.Response ) {
					HttpWebResponse curResp = (HttpWebResponse)response;
					if ( curResp == null ) {
						Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, e.Message ) );
						MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection on request for IWWF license: {1}", curMethodName, e.Message ) );
						return null;
					}
                    if ( curResp.StatusCode == HttpStatusCode.Accepted ) {
                        String respMsg = getResponseAsString( curResp );
                        return jsonSerializer.Deserialize<Dictionary<string, object>>( respMsg );
                    }
                    if ( curResp.StatusCode == HttpStatusCode.NotFound ) {
                        Log.WriteFile( String.Format( "{0}Target location not found, {1}", curMethodName, e.Message ) );
                        MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection on request for IWWF license: {1}", curMethodName, e.Message ) );
                        return null;
                    }
                    if ( curResp.StatusCode == HttpStatusCode.BadRequest ) {
                        String respMsg = getResponseAsString( curResp );
                        return jsonSerializer.Deserialize<Dictionary<string, object>>( respMsg );
                    }

                    Log.WriteFile( String.Format( "{0}Request failed, {1}", curMethodName, e.Message ) );
                    MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection on request for IWWF license: {1}", curMethodName, e.Message ) );
                    return null;
                }

            } catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );
				return null;
            }
        }
		
		public static Dictionary<string, object> deleteMessagePostJsonResp(String inUrl, NameValueCollection inHeaderParams, String inContentType, String inMessage) {
            String curMethodName = "SendMessageHttp:deleteMessagePostJsonResp: ";
            HttpWebRequest curRequest = null;
            Stream curDataStream = null;

            try {
                // Create a request using a URL that can receive a post. 
                // Set the Method property of the request to POST.
                curRequest = (HttpWebRequest)WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;
				curRequest.Method = "DELETE";
                curRequest.KeepAlive = false;
                curRequest.ContentType = inContentType;

                //Set header parameters to the WebRequest
                ( (HttpWebRequest)curRequest ).UserAgent = ".NET Framework CustomUserAgent Water Ski Scoring";
                if (inHeaderParams != null) {
                    foreach (string curKey in inHeaderParams.Keys) {
                        curRequest.Headers[curKey] = inHeaderParams[curKey];
                    }
                }

                // Create POST data and convert it to a byte array.
                if (inMessage != null) {
                    byte[] byteArray = Encoding.UTF8.GetBytes( inMessage );

                    // Set the ContentLength property of the WebRequest.
                    curRequest.ContentLength = byteArray.Length;

                    // Get the request stream.
                    curDataStream = curRequest.GetRequestStream();

                    // Write the data to the request stream.
                    curDataStream.Write( byteArray, 0, byteArray.Length );

                    // Close the Stream object.
                    curDataStream.Close();
                }

				HttpWebResponse curResp = ( HttpWebResponse)curRequest.GetResponse();
				JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
				jsonSerializer.MaxJsonLength = Int32.MaxValue;
				return jsonSerializer.Deserialize<Dictionary<string, object>>( getResponseAsString( curResp ) );

            } catch (Exception ex) {
				Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
				MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );
				return null;
            }
        }

        public static bool sendMessagePostWithHeader(String inUrl, String inAuthHeaderParms, String inContentType, String inMessage) {
            String curMethodName = "SendMessageHttp:sendMessagePostWithHeader: ";
            HttpWebRequest curRequest = null;
            RequestState curRequestState = null;

            try {
                // Create a request using a URL that can receive a post. 
                // Set the Method property of the request to POST.
                curRequest = (HttpWebRequest)WebRequest.Create( inUrl );
				curRequest.AllowAutoRedirect = false;
				curRequest.Method = "POST";
                curRequest.KeepAlive = false;
                curRequest.ContentType = inContentType;
                if (inAuthHeaderParms != null) {
                    curRequest.Headers.Add( "Authorization", inAuthHeaderParms );
                }

                curRequestState = new RequestState();
                curRequestState.WebReqst = curRequest;
                curRequestState.InputMsgBuffer = Encoding.UTF8.GetBytes( inMessage );
                curRequestState.WebReqst.ContentLength = curRequestState.InputMsgBuffer.Length;

                // Get the request stream.
                //curDataStream = curRequest.BeginGetRequestStream.GetRequestStream();
                curRequest.BeginGetRequestStream( new AsyncCallback( GetRequestStreamCallback ), curRequestState );

            } catch (Exception ex) {
				MessageBox.Show( String.Format( "{0} Likely temporary loss of internet connection {1}", curMethodName, ex.Message ) );
                return false;
            }

            return true;
        }
        public static bool sendMessagePostXml(String inUrl, String inMessage) {
            return sendMessagePostAsync( inUrl, inMessage, "text/xml;charset=\"utf-8\"", null, null );
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

		public static DataTable readJsonResponseAsDataTable(HttpWebResponse curResponse) {
			String curMethodName = "readJsonResponseAsDataTable: ";

			Stream curResponseStream = null;
			StreamReader curStreamReader = null;
			JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
			jsonSerializer.MaxJsonLength = Int32.MaxValue;

			try {
				curResponseStream = curResponse.GetResponseStream();
				curStreamReader = new StreamReader(curResponseStream);
				String curResponseMessage = curStreamReader.ReadToEnd();
				if (curResponseMessage.Length > 0) {
					return convertDictionaryListToDataTable( jsonSerializer.Deserialize<List<object>>(curResponseMessage) );
				}
				return null;

			} catch (Exception ex) {
				MessageBox.Show(curMethodName + ":Exception:" + ex.Message);
				Log.WriteFile(curMethodName + ":Exception:" + ex.Message);
				return null;

			} finally {
				// Clean up the streams.
				if (curStreamReader != null) curStreamReader.Close();
				if (curResponseStream != null) curResponseStream.Close();
				if (curResponse != null) curResponse.Close();
			}
		}

		public static List<object> readJsonResponseAsList(HttpWebResponse curResponse) {
			String curMethodName = "readJsonResponseAsDataTable: ";

			Stream curResponseStream = null;
			StreamReader curStreamReader = null;
			JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
			jsonSerializer.MaxJsonLength = Int32.MaxValue;

			try {
				curResponseStream = curResponse.GetResponseStream();
				curStreamReader = new StreamReader(curResponseStream);
				String curResponseMessage = curStreamReader.ReadToEnd();
				if (curResponseMessage.Length > 0) {
					return jsonSerializer.Deserialize<List<object>>(curResponseMessage);
				}
				return null;

			} catch (Exception ex) {
				MessageBox.Show(curMethodName + ":Exception:" + ex.Message);
				Log.WriteFile(curMethodName + ":Exception:" + ex.Message);
				return null;

			} finally {
				// Clean up the streams.
				if (curStreamReader != null) curStreamReader.Close();
				if (curResponseStream != null) curResponseStream.Close();
				if (curResponse != null) curResponse.Close();
			}
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

		public static List<KeyValuePair<String, String>> readJsonResponse(String inResponse) {
            List<KeyValuePair<String, String>> curResponseDataList = null;

            if (inResponse.Length > 0) {
                int curDelimBegin = inResponse.IndexOf( "{" );
                int curDelimEnd = inResponse.LastIndexOf( "}" );
                if (curDelimBegin >= 0 && curDelimEnd >= 0) {
                    curResponseDataList = new List<KeyValuePair<String, String>>();
                    String[] curTempDataList = inResponse.Substring( curDelimBegin + 1, curDelimEnd - curDelimBegin ).Split( ',' );
                    foreach (String curEntry in curTempDataList) {
                        curDelimBegin = curEntry.IndexOf( ":" );
                        if (curDelimBegin > 0) {
                            curResponseDataList.Add( new KeyValuePair<String, String>( curEntry.Substring( 0, curDelimBegin ).Replace( "\"", "" ), curEntry.Substring( curDelimBegin + 1 ).Replace( "\"", "" ) ) );
                        } else {
                            //MessageBox.Show( "Bypassing current entry because it is not recognized: " + curEntry );
                        }
                    }
                } else {
                    curResponseDataList = new List<KeyValuePair<String, String>>();
                    String[] curTempDataList = inResponse.Split( '&' );
                    foreach (String curEntry in curTempDataList) {
                        String[] tmpEntry = curEntry.Split( '=' );
                        if (tmpEntry.Length == 2) {
                            curResponseDataList.Add( new KeyValuePair<String, String>( tmpEntry[0], tmpEntry[1] ) );
                        } else {
                            //MessageBox.Show( "Bypassing current entry because it is not recognized: " + curEntry );
                        }
                    }
                }
            }
            return curResponseDataList;
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

        private static bool showServerResp(String inMsg) {
            // Display the content.
            String curMethodName = "SendMessageHttp:showServerResp: ";
            String curMsgShow = "";
            int curIdx = 0;
            String[] curRespMsg = inMsg.Split( '\n' );
            while (curIdx < curRespMsg.Length) {
                for (int curCount = 0; curCount < 50 && curIdx < curRespMsg.Length; curCount++) {
                    curMsgShow += curRespMsg[curIdx] + "\n";
                    curIdx++;
                    curCount++;
                }
                MessageBox.Show( curMsgShow );
                Log.WriteFile( curMethodName + ":" + curMsgShow );
                curMsgShow = "";
            }
            return true;
        }

        public static bool showServerRespJson(Dictionary<string, object> inMsgDataList) {
            if (inMsgDataList.Count > 0) {
                int curCount = 0;
                StringBuilder curDispMessage = new StringBuilder( "" );
                foreach (KeyValuePair<String, Object> curEntry in inMsgDataList) {
                    curCount++;
                    curDispMessage.Append( curEntry.Key + "=> " + curEntry.Value + "\n" );
                    if (curCount > 20) {
                        MessageBox.Show( curDispMessage.ToString() );
                        curDispMessage = new StringBuilder( "" );
                        curCount = 0;
                    }
                }
                if (curCount > 0) {
                    MessageBox.Show( curDispMessage.ToString() );
                    curDispMessage = new StringBuilder( "" );
                }
            }
            return true;
        }

        public static bool showServerRespJson(List<KeyValuePair<String, String>> inMsgDataList) {
            if (inMsgDataList.Count > 0) {
                int curCount = 0;
                StringBuilder curDispMessage = new StringBuilder( "" );
                foreach (KeyValuePair<String, String> curEntry in inMsgDataList) {
                    curCount++;
                    curDispMessage.Append( curEntry.Key + "=> " + curEntry.Value + "\n" );
                    if (curCount > 20) {
                        MessageBox.Show( curDispMessage.ToString() );
                        curDispMessage = new StringBuilder( "" );
                        curCount = 0;
                    }
                }
                if (curCount > 20) {
                    MessageBox.Show( curDispMessage.ToString() );
                    curDispMessage = new StringBuilder( "" );
                }
            }

            return true;
        }

        private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            //solve the problem of invalid certificates - accept all as valid
            return true;
        }

		/*
        * Concvert a list of dictionary entries into a data table
        */
		public static DataTable convertDictionaryListToDataTable( List<object> curDataList ) {
			String curMethodName = "ImportOfficialRatings:convertDictionaryListToDataTable: ";
			DataTable returnDatatTable = new DataTable();

			if ( curDataList.Count() > 0 ) {
				Dictionary<string, object> dataHeaderEntry = (Dictionary<string, object>) curDataList.ElementAt( 0 );
				foreach ( KeyValuePair<String, object> curEntry in dataHeaderEntry ) {
					returnDatatTable.Columns.Add( curEntry.Key );
				}
			}

			foreach ( Dictionary<string, object> curEntry in curDataList ) {
				DataRow dataRow = returnDatatTable.NewRow();

				foreach ( KeyValuePair<String, object> curEntryAttr in curEntry ) {
					try {
						dataRow[curEntryAttr.Key] = curEntryAttr.Value;

					} catch ( Exception ex ) {
						Log.WriteFile( String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message ) );
						MessageBox.Show( String.Format( "{0}Likely temporary loss of internet connection: {1}", curMethodName, ex.Message ) );
					}
				}

				returnDatatTable.Rows.Add( dataRow );
			}

			return returnDatatTable;
		}

		/*
		 */
		public static Dictionary<string, object> convertDataRowToDictionary(DataRow curDataRow) {
			Dictionary<string, object> curMemberData = new Dictionary<string, object>(); ;
			foreach (DataColumn dataColumn in curDataRow.Table.Columns) {
				try {
					if (curDataRow[dataColumn.ColumnName] != System.DBNull.Value) {
						curMemberData.Add(dataColumn.ColumnName, curDataRow[dataColumn.ColumnName]);
					}
				} catch (Exception ex) {
					String curValue = ex.Message;
					if (curValue.Length > 0) {
						String curValueX = curValue;
					}
				}
			}
			return curMemberData;
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

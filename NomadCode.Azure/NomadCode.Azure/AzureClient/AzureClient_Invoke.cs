using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

using Newtonsoft.Json.Linq;

namespace NomadCode.Azure
{
    public partial class AzureClient // Invoke
    {

        public async Task<U> InvokeApiAsync<T, U> (string apiName, T body, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync<T, U> (apiName, body, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync<T, U> (apiName, body, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return default (U);
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return default (U);
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<HttpResponseMessage> InvokeApiAsync (string apiName, HttpContent content, HttpMethod method, IDictionary<string, string> requestHeaders, IDictionary<string, string> parameters)
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync (apiName, content, method, requestHeaders, parameters);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync (apiName, content, method, requestHeaders, parameters);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return null;
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return null;
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<HttpResponseMessage> InvokeApiAsync (string apiName, HttpContent content, HttpMethod method, IDictionary<string, string> requestHeaders, IDictionary<string, string> parameters, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync (apiName, content, method, requestHeaders, parameters, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync (apiName, content, method, requestHeaders, parameters, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return null;
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return null;
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<JToken> InvokeApiAsync (string apiName, JToken body, HttpMethod method, IDictionary<string, string> parameters, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync (apiName, body, method, parameters, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync (apiName, body, method, parameters, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return null;
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return null;
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<JToken> InvokeApiAsync (string apiName, HttpMethod method, IDictionary<string, string> parameters, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync (apiName, method, parameters, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync (apiName, method, parameters, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return null;
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return null;
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<T> InvokeApiAsync<T> (string apiName, HttpMethod method, IDictionary<string, string> parameters, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync<T> (apiName, method, parameters, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync<T> (apiName, method, parameters, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return default(T);
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return default (T);
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<JToken> InvokeApiAsync (string apiName, JToken body, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync (apiName, body, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync (apiName, body, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return null;
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return null;
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<U> InvokeApiAsync<T, U> (string apiName, T body, HttpMethod method, IDictionary<string, string> parameters, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync<T, U> (apiName, body, method, parameters, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync<T, U> (apiName, body, method, parameters, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return default(U);
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return default (U);
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<JToken> InvokeApiAsync (string apiName, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync (apiName, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync (apiName, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return null;
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");

                return null;
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }


        public async Task<T> InvokeApiAsync<T> (string apiName, CancellationToken cancellationToken = default (CancellationToken))
        {
#if DEBUG
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
#endif
            try
            {
                return await Client.InvokeApiAsync<T> (apiName, cancellationToken);
            }
            catch (MobileServiceInvalidOperationException ioEx)
            {
                if (ioEx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reauth = await ReAuthenticateAsync ();

                    if (reauth)
                    {
                        return await Client.InvokeApiAsync<T> (apiName, cancellationToken);
                    }
                }

                throw;
#if !DEBUG
            }
            catch (Exception)
            {
                return default(T);
#else
            }
            catch (Exception e)
            {
                logDebug ($"InvokeApiAsync ({apiName}) failed : {(e.InnerException ?? e).Message}");
                return default (T);
            }
            finally
            {
                sw.Stop ();

                logDebug ($"InvokeApiAsync ({apiName}) took {sw.ElapsedMilliseconds} milliseconds");
#endif
            }
        }
    }
}



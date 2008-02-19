using System;
using System.Collections.Specialized;
using System.Text;
using System.Net;

namespace DotNetOpenId.Provider
{
    /// <summary>
    /// Could not encode this as a protocol message.
    /// </summary>
    public class EncodingException : Exception
    {
        internal EncodingException(IEncodable response)
        {
            Response = response;
        }

        internal IEncodable Response { get; private set; }
    }

    /// <summary>
    /// This response is already signed.
    /// </summary>
    public class AlreadySignedException : EncodingException
    {
        internal AlreadySignedException(IEncodable response)
            : base(response)
        {
        }
    }

    /// <summary>
    /// Encodes responses in to <see cref="WebResponse"/>.
    /// </summary>
    internal class Encoder
    {
        /// <summary>
        /// Encodes responses in to WebResponses.
        /// </summary>
        public virtual WebResponse Encode(IEncodable response)
        {
            EncodingType encode_as = response.EncodingType;
            WebResponse wr;

            #region  Trace
            if (TraceUtil.Switch.TraceInfo)
            {
                TraceUtil.ProviderTrace(String.Format("Encode using {0}", encode_as));
            }
            #endregion

            switch (encode_as)
            {
                case EncodingType.ResponseBody:
                    HttpStatusCode code = (response is Exception) ? 
                        HttpStatusCode.BadRequest : HttpStatusCode.OK;
                    wr = new WebResponse(code, null, DictionarySerializer.Serialize(response.EncodedFields));
                    break;
                case EncodingType.RedirectBrowserUrl:
                    NameValueCollection headers = new NameValueCollection();

                    UriBuilder builder = new UriBuilder(response.BaseUri);
                    UriUtil.AppendQueryArgs(builder, response.EncodedFields);

                    headers.Add("Location", builder.Uri.AbsoluteUri);

                    wr = new WebResponse(HttpStatusCode.Redirect, headers, new byte[0]);
                    break;
                default:
                    throw new EncodingException(response);
            }
            return wr;
        }
    }

    /// <summary>
    /// Encodes responses in to <see cref="WebResponse"/>, signing them when required.
    /// </summary>
    internal class SigningEncoder : Encoder
    {
        Signatory signatory;

        public SigningEncoder(Signatory signatory)
        {
            this.signatory = signatory;
        }

        public override WebResponse Encode(IEncodable encodable)
        {
            if (!(encodable is Exception))
            {
                #region  Trace
                if (TraceUtil.Switch.TraceInfo)
                {
                    TraceUtil.ProviderTrace("Encoding using the signing encoder");
                }
                #endregion                          
                
                
                Response response = (Response)encodable;

                if (response.NeedsSigning)
                {
                    if (signatory == null)
                        throw new ArgumentException("Must have a store to sign this request");

                    if (response.Fields.ContainsKey(QueryStringArgs.openidnp.sig))
                        throw new AlreadySignedException(encodable);

                    signatory.Sign(response);
                }

            }

            return base.Encode(encodable);
        }
    }
}

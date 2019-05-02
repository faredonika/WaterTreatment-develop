using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using WaterTreatment.Web.Services.Interface;

namespace WaterTreatment.Web.Services
{
    public class AWSS3 : IAWSS3
    {
        private static IAmazonS3 client;

        public string CreateBucket()
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            using (AWSS3.client = new AmazonS3Client())
            {
                try
                {
                    if (!AmazonS3Util.DoesS3BucketExist(AWSS3.client, appSettings["BucketName"]))
                    {
                        PutBucketRequest request = new PutBucketRequest
                        {
                            BucketName = appSettings["BucketName"],
                            UseClientRegion = true
                        };
                        AWSS3.client.PutBucket(request);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.ErrorCode != null && (ex.ErrorCode.Equals("InvalidAccessKeyId") || ex.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Console.WriteLine("Check the provided AWS Credentials.");
                        Console.WriteLine("For service sign up go to http://aws.amazon.com/s3");
                    }
                    else
                    {
                        Console.WriteLine("Error occurred. Message:'{0}' when writing an object", ex.Message);
                    }
                }
            }
            return appSettings["BucketName"];
        }

        public Guid UploadFile(Stream data)
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            string bucketName = this.CreateBucket();
            Guid result = Guid.NewGuid();
            string path = appSettings["AppStoragePath"];

            using (TransferUtility transferUtility = new TransferUtility(new AmazonS3Client()))
            {
                transferUtility.Upload(data, bucketName, path + result.ToString());
            }
            return result;
        }

        public Stream GetFile(Guid key)
        {
            Stream result;
            try
            {
                using (AWSS3.client = new AmazonS3Client())
                {
                    NameValueCollection appSettings = ConfigurationManager.AppSettings;
                    MemoryStream memoryStream = new MemoryStream();
                    string path = appSettings["AppStoragePath"];

                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = appSettings["BucketName"],
                        Key = path + key.ToString()
                    };

                    using (GetObjectResponse @object = AWSS3.client.GetObject(request))
                    {
                        using (Stream responseStream = @object.ResponseStream)
                        {
                            responseStream.CopyTo(memoryStream);
                        }
                    }
                    memoryStream.Position = 0L;
                    result = memoryStream;
                }
            }
            catch (Exception ex )
            {
                string BannerMessage = ex.Message;
                result = null;
            }
            return result;
        }

        public void Update(Guid key, Stream data)
        {
            string bucketName = ConfigurationManager.AppSettings["BucketName"];
            string path = ConfigurationManager.AppSettings["AppStoragePath"];

            using (TransferUtility transferUtility = new TransferUtility(new AmazonS3Client()))
            {
                transferUtility.Upload(data, bucketName, path + key.ToString());
            }
        }

        public void Remove(Guid key)
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            string path = appSettings["AppStoragePath"];

            using (AWSS3.client = new AmazonS3Client())
            {
                DeleteObjectRequest request = new DeleteObjectRequest
                {
                    BucketName = appSettings["BucketName"],
                    Key = path + key.ToString()
                };
                AWSS3.client.DeleteObject(request);
            }
        }
    }
}
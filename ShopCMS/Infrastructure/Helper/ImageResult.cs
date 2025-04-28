using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ahmadi.Infrastructure.Helper
{
    public class ImageResult : ActionResult
    {
        private readonly Image _image;

        public ImageResult(Image image)
        {
            _image = image;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            try
            {
                _image.Save(response.OutputStream, ImageFormat.Jpeg);
            }
            finally
            {
                _image.Dispose();
            }
        }
    }

    public static class ImageExtensions
    {
        public static Image BestFit(this Image image, int? height, int? width)
        {
            return image.Height > image.Width
                ? image.ConstrainProportions(height, Dimensions.Height)
                : image.ConstrainProportions(width, Dimensions.Width);
        }

        public static Image ConstrainProportions(this Image imgPhoto, int? size, Dimensions dimension)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;
            float nPercent;

            switch (dimension)
            {
                case Dimensions.Width:
                    nPercent = (float)(size.HasValue ? size.Value : 1280) / (float)sourceWidth;
                    break;
                default:
                    nPercent = (float)(size.HasValue ? size.Value : 1280) / (float)sourceHeight;
                    break;
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.CompositingQuality = CompositingQuality.HighSpeed;
            grPhoto.InterpolationMode = InterpolationMode.Default;
            grPhoto.SmoothingMode = SmoothingMode.HighSpeed;
            
            try
            {
                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel

                );

                //ImageCodecInfo imageCodecInfo;
                //switch (extention)
                //{
                //    case ".jpg":
                //    case ".jpeg":
                //        imageCodecInfo = CoreLib.Infrastructure.Image.ImageClass.GetEncoderInfo(ImageFormat.Jpeg);
                //        break;
                //    case ".png":
                //        imageCodecInfo = CoreLib.Infrastructure.Image.ImageClass.GetEncoderInfo(ImageFormat.Png);
                //        break;
                //    case ".gif":
                //        imageCodecInfo = CoreLib.Infrastructure.Image.ImageClass.GetEncoderInfo(ImageFormat.Gif);
                //        break;
                //    case ".bmp":
                //        imageCodecInfo = CoreLib.Infrastructure.Image.ImageClass.GetEncoderInfo(ImageFormat.Bmp);
                //        break;
                //    case ".tif":
                //        imageCodecInfo = CoreLib.Infrastructure.Image.ImageClass.GetEncoderInfo(ImageFormat.Tiff);
                //        break;
                //    default:
                //        imageCodecInfo = CoreLib.Infrastructure.Image.ImageClass. GetEncoderInfo(ImageFormat.Jpeg);
                //        break;
                //}

                ////Create an Encoder object for the Quality parameter.

                //Encoder encoder = Encoder.Compression;

                //// Create an EncoderParameters object. 
                //EncoderParameters encoderParameters = new EncoderParameters(1);

                //// Save the image as a JPEG file with quality level.
                //EncoderParameter encoderParameter = new EncoderParameter(encoder, 1);
                //encoderParameters.Param[0] = encoderParameter;
                //imgPhoto.Save(filePath, imageCodecInfo, encoderParameters);
            }
            finally
            {
                grPhoto.Dispose();
            }

            return bmPhoto;
        }


        public static byte[] ConvertToByteArray(this Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

    public enum Dimensions
    {
        Height,
        Width
    }
}

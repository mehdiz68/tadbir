using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace CoreLib.Infrastructure.Image
{

    public class ImageClass
    {
        public ImageClass()
        {

        }

        public static Boolean IsFileAnImage(string strFileName)
        {
            Boolean result = true;
            try
            {
                if (strFileName.IndexOf("http") < 0)
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(strFileName));
                    img.Dispose();
                }
                else
                    result = true;

            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static string WaterMarkAndChangeSize(string path, string source_file, bool UseWatermark, bool ChangeSize, bool compression, Int16 compressionLevel, bool LargeWatermark, string WatermarkFileName = "", string WatermarkPosition = "")
        {
            Bitmap mSourceImage = default(Bitmap);
            string fileName = null;
            try
            {
                using (mSourceImage = (Bitmap)System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(path + source_file)))
                {


                    fileName = HttpContext.Current.Server.MapPath(path + source_file);
                    string extention = source_file.Substring(source_file.LastIndexOf(".")).ToLower();
                    // save resized image to destination Folder
                    if (UseWatermark)
                    {
                        Graphics g = Graphics.FromImage(mSourceImage);
                        Bitmap WaterMarkPic = new Bitmap(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + WatermarkFileName));
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        #region Rezie Watermak if Needed
                        if (WaterMarkPic.Width >= mSourceImage.Width || WaterMarkPic.Height >= mSourceImage.Height)
                        {
                            //decrease To 0.3 of Source Image
                            WaterMarkPic = resizeImage(WaterMarkPic, new Size(Convert.ToInt32(Math.Ceiling(mSourceImage.Width * 0.3)), Convert.ToInt32(Math.Ceiling(mSourceImage.Height * 0.3))), true, extention);

                        }
                        else
                        {
                            double ScaleX = mSourceImage.Width / WaterMarkPic.Width;
                            double ScaleY = mSourceImage.Height / WaterMarkPic.Width;
                            if (ScaleX < 3 || ScaleY < 3)
                            {
                                //decrease To 0.3 of Source Image
                                WaterMarkPic = resizeImage(WaterMarkPic, new Size(Convert.ToInt32(Math.Ceiling(mSourceImage.Width * 0.3)), Convert.ToInt32(Math.Ceiling(mSourceImage.Height * 0.3))), true, extention);
                            }

                        }
                        #endregion
                        #region Get X,Y For Start Position
                        int startX = 0, startY = 0;
                        if (WatermarkPosition != "")
                            startX = CalculateWatermarkPosition(mSourceImage.Width, mSourceImage.Height, Convert.ToInt32(WatermarkPosition), WaterMarkPic.Width, WaterMarkPic.Height, out startY);
                        #endregion
                        #region Save Watermark Temporary
                        g.DrawImage(WaterMarkPic, new Point(startX, startY));
                        g.Save();
                        WaterMarkPic.Dispose();
                        g.Dispose();
                        string fileOut = Path.Combine(HttpContext.Current.Server.MapPath(path), source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf(".")));
                        FileStream ms = new FileStream(fileOut, FileMode.Create, FileAccess.Write);
                        mSourceImage.Save(ms, mSourceImage.RawFormat);
                        ms.Flush();
                        ms.Close();
                        ms.Dispose();
                        #endregion
                    }
                    #region compression
                    if (compression)
                    {
                        if (extention == ".jpg" || extention == ".jpeg" || extention == ".png")
                        {
                            Compress(mSourceImage, HttpContext.Current.Server.MapPath(path), source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf(".")), compressionLevel);
                            mSourceImage = (Bitmap)System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))));
                        }
                    }
                    #endregion
                    #region Multi Size
                    if (ChangeSize)
                    {
                        if (LargeWatermark)
                        {
                            mSourceImage = (Bitmap)System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(path + source_file));
                        }
                        switch (extention)
                        {
                            case ".jpg":
                            case ".jpeg":
                                //resizeImage(mSourceImage, new Size(mSourceImage.Width, mSourceImage.Height), false, extention).Save(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 2, mSourceImage.Height / 2), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 4, mSourceImage.Height / 4), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 8, mSourceImage.Height / 8), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            case ".png":
                                resizeImage(mSourceImage, new Size(mSourceImage.Width, mSourceImage.Height), false, extention).Save(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), System.Drawing.Imaging.ImageFormat.Png);
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 2, mSourceImage.Height / 2), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 4, mSourceImage.Height / 4), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 8, mSourceImage.Height / 8), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            case ".gif":
                                resizeImage(mSourceImage, new Size(mSourceImage.Width, mSourceImage.Height), false, extention).Save(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), System.Drawing.Imaging.ImageFormat.Gif);
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 2, mSourceImage.Height / 2), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 4, mSourceImage.Height / 4), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 8, mSourceImage.Height / 8), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                                break;
                            case ".bmp":
                                resizeImage(mSourceImage, new Size(mSourceImage.Width, mSourceImage.Height), false, extention).Save(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), System.Drawing.Imaging.ImageFormat.Bmp);
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 2, mSourceImage.Height / 2), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 4, mSourceImage.Height / 4), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 8, mSourceImage.Height / 8), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            case ".tif":
                                resizeImage(mSourceImage, new Size(mSourceImage.Width, mSourceImage.Height), false, extention).Save(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), System.Drawing.Imaging.ImageFormat.Tiff);
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 2, mSourceImage.Height / 2), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 4, mSourceImage.Height / 4), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 8, mSourceImage.Height / 8), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                                break;
                            default:
                                resizeImage(mSourceImage, new Size(mSourceImage.Width, mSourceImage.Height), false, extention).Save(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 2, mSourceImage.Height / 2), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 4, mSourceImage.Height / 4), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                resizeImage(mSourceImage, new Size(mSourceImage.Width / 8, mSourceImage.Height / 8), false, extention).Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                        }

                    }
                }
                #endregion
                if (UseWatermark || compression || ChangeSize)
                {
                    #region Save Final Image
                    mSourceImage.Dispose();
                    //if (!ChangeSize)
                    //{
                    File.Delete(HttpContext.Current.Server.MapPath(path + source_file));
                    //}
                    File.Move(HttpContext.Current.Server.MapPath(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."))), HttpContext.Current.Server.MapPath(path + source_file));

                    #endregion
                }
                else
                    mSourceImage.Dispose();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static Bitmap resizeImage(Bitmap imgToResize, Size size, bool ForWatermakr, string extention)
        {
            // Get the image's original width and height
            int originalWidth = imgToResize.Width;
            int originalHeight = imgToResize.Height;

            // To preserve the aspect ratio
            float ratioX = (float)size.Width / (float)originalWidth;
            float ratioY = (float)size.Height / (float)originalHeight;
            //float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratioX);
            int newHeight = (int)(originalHeight * ratioY);

            // Convert other formats (including CMYK) to RGB
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            newImage.SetResolution(72, 72);
            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.DrawImage(imgToResize, 0, 0, newWidth, newHeight);
            }
            if (ForWatermakr)
                imgToResize.Dispose();

            // Get an ImageCodecInfo object that represents .
            //ImageCodecInfo imageCodecInfo;
            //switch (extention)
            //{
            //    case ".jpg":
            //    case ".jpeg":
            //         imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            //        break;
            //    case ".png":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Png);
            //        break;
            //    case ".gif":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Gif);
            //        break;
            //    case ".bmp":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Bmp);
            //        break;
            //    case ".tif":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Tiff);
            //        break;
            //    default:
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            //        break;
            //}

            // Create an Encoder object for the Quality parameter.
            //Encoder encoder = Encoder.Quality;

            //// Create an EncoderParameters object. 
            //EncoderParameters encoderParameters = new EncoderParameters(1);

            //// Save the image as a JPEG file with quality level.
            //EncoderParameter encoderParameter = new EncoderParameter(encoder, 1);
            //encoderParameters.Param[0] = encoderParameter;
            ////newImage.Save(filePath, imageCodecInfo, encoderParameters);
            return newImage;

            //int sourceWidth = imgToResize.Width;
            //int sourceHeight = imgToResize.Height;

            ////float nPercent = 0;
            //float nPercentW = 0;
            //float nPercentH = 0;

            //nPercentW = ((float)size.Width / (float)sourceWidth);
            //nPercentH = ((float)size.Height / (float)sourceHeight);

            ////if (nPercentH < nPercentW)
            ////    nPercent = nPercentH;
            ////else
            ////    nPercent = nPercentW;

            //int destWidth = (int)(sourceWidth * nPercentW);
            //int destHeight = (int)(sourceHeight * nPercentH);

            //Bitmap b = new Bitmap(destWidth, destHeight);
            //Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            //g.InterpolationMode = InterpolationMode.Default;

            //g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            //if(ForWatermakr)
            //    imgToResize.Dispose();
            //g.Dispose();

            //return b;
        }
        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        private static int CalculateWatermarkPosition(int imageWidth, int imageHeight, int WatermarkPosition, int watermarkImageWidth, int watermarkImageHeight, out int y)
        {
            try
            {
                switch (WatermarkPosition)
                {
                    // Y=ImageHeight-WaterMarkHeight- 10% Of ImageHeight , X= 2% of ImageWidth 
                    case 0: y = imageHeight - watermarkImageHeight - Convert.ToInt32(imageHeight * 0.02); return Convert.ToInt32(imageWidth * 0.02);
                    // Y=ImageHeight-WaterMarkHeight- 10% Of ImageHeight , X=(ImageWidth / 2) - (watermarkImageWidth / 2)
                    case 1: y = imageHeight - watermarkImageHeight - Convert.ToInt32(imageHeight * 0.02); return (imageWidth / 2) - (watermarkImageWidth / 2);
                    //Y=ImageHeight-WaterMarkHeight- 10% Of ImageHeight , X=(ImageWidth / 2) - (watermarkImageWidth / 2) - 2% of ImageWidth 
                    case 2: y = imageHeight - watermarkImageHeight - Convert.ToInt32(imageHeight * 0.02); return imageWidth - watermarkImageWidth - Convert.ToInt32(imageWidth * 0.02);
                    // Y=(imageHeight / 2) - (watermarkImageHeight / 2) , X= 2% of ImageWidth 
                    case 3: y = (imageHeight / 2) - (watermarkImageHeight / 2); return Convert.ToInt32(imageWidth * 0.02);
                    // Y=(ImageHeight / 2) - (watermarkImageHeight / 2) , X= (ImageWidth / 2) - (watermarkImageWidth / 2) 
                    case 4: y = (imageHeight / 2) - (watermarkImageHeight / 2); return (imageWidth / 2) - (watermarkImageWidth / 2);
                    //Y=Y=(ImageHeight / 2) - (watermarkImageHeight / 2) , X=(ImageWidth / 2) - (watermarkImageWidth / 2) - 2% of ImageWidth 
                    case 5: y = (imageHeight / 2) - (watermarkImageHeight / 2); return imageWidth - watermarkImageWidth - Convert.ToInt32(imageWidth * 0.02);
                    // Y=ImageHeight * 0.02 , X= ImageWidth * 0.02
                    case 6: y = Convert.ToInt32(imageHeight * 0.02); return Convert.ToInt32(imageWidth * 0.02);
                    // Y=ImageHeight * 0.02 ,  X=(ImageWidth / 2) - (watermarkImageWidth / 2)
                    case 7: y = Convert.ToInt32(imageHeight * 0.02); return (imageWidth / 2) - (watermarkImageWidth / 2);
                    // Y=ImageHeight * 0.02 ,  X=X=(ImageWidth / 2) - (watermarkImageWidth / 2) - 2% of ImageWidth
                    case 8: y = Convert.ToInt32(imageHeight * 0.02); return imageWidth - watermarkImageWidth - Convert.ToInt32(imageWidth * 0.02);
                    default: y = 1; return 1;
                }
            }
            catch (Exception)
            {
                y = 0;
                return 0;
            }
        }

        static void Compress(System.Drawing.Image original, string path, string filename, int compressionLevel)
        {
            ImageCodecInfo jpgEncoder = null;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpgEncoder = codec;
                    break;
                }
            }
            if (jpgEncoder != null)
            {
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);

                EncoderParameter encoderParameter;
                switch (compressionLevel)
                {
                    case 1: encoderParameter = new EncoderParameter(encoder, 5L); break;
                    case 2: encoderParameter = new EncoderParameter(encoder, 10L); break;
                    case 3: encoderParameter = new EncoderParameter(encoder, 15L); break;
                    case 4: encoderParameter = new EncoderParameter(encoder, 20L); break;
                    case 5: encoderParameter = new EncoderParameter(encoder, 25L); break;
                    case 6: encoderParameter = new EncoderParameter(encoder, 30L); break;
                    case 7: encoderParameter = new EncoderParameter(encoder, 35L); break;
                    case 8: encoderParameter = new EncoderParameter(encoder, 40L); break;
                    case 9: encoderParameter = new EncoderParameter(encoder, 50L); break;
                    default:
                        encoderParameter = new EncoderParameter(encoder, 40L);
                        break;
                }
                encoderParameters.Param[0] = encoderParameter;

                string fileOut = Path.Combine(path, filename);
                FileStream ms = new FileStream(fileOut, FileMode.Create, FileAccess.Write);
                original.Save(ms, jpgEncoder, encoderParameters);
                //Bitmap compressed = new Bitmap(ms);
                ms.Flush();
                ms.Close();
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using Sa3adaty.DAL.Infrastructure;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Sa3adaty.Core.Services
{
    public  class ImageService
    {
        
        #region Methods
        public static bool IsValid(IEnumerable<HttpPostedFileBase> images)
        {
            foreach (HttpPostedFileBase file in images )
            {
                if (file == null)
                    continue;
                //check file type
                if (file.ContentType.ToLower() != "image/jpg" &&
                       file.ContentType.ToLower() != "image/jpeg" &&
                       file.ContentType.ToLower() != "image/png")
                {
                    return false;
                }

                //check files size
                int MaxContentLength = 1024 * 1024 * 3; //3 MB
                if (file.ContentLength > MaxContentLength)
                    return false;
            }
            return true;
        }

        public static bool IsValid(HttpPostedFileBase image)
        {
            if (image == null)
                return false;

            if (image.ContentType.ToLower() != "image/jpg" &&
                       image.ContentType.ToLower() != "image/jpeg" &&
                       image.ContentType.ToLower() != "image/png")
                {
                    return false;
                }

            //check files size
            int MaxContentLength = 1024 * 1024 * 3; //3 MB
            if (image.ContentLength > MaxContentLength)
                return false;
            
            return true;
        }

        public static bool SaveImage(Image image, string file_name, int width = 0, int height = 0, string directory_path = null)
        {

            string directory = "";
            if (directory_path == null)
            {
                string image_directory = GetImagesDirectory();
                directory = HttpContext.Current.Server.MapPath("~" + image_directory);
            }
            else
            {
                directory = directory_path;
            }

            try
            {
                if (width == 0 || height == 0)
                {
                    //Save the original image

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    image.Save(directory + "/" + file_name);
                }
                else
                {
                    //save resized/cropped images (thumbnails)
                    image = ResizeImage(image, width , height );

                   ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID 
                    // for the Quality parameter category.
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object. 
                    // An EncoderParameters object has an array of EncoderParameter 
                    // objects. In this case, there is only one 
                    // EncoderParameter object in the array.
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
                    myEncoderParameters.Param[0] = myEncoderParameter;


                    image.Save(directory + "/" + GenerateImageFileName(file_name, width.ToString(), height.ToString()),jpgEncoder,myEncoderParameters );
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static  ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        public static Image ResizeImage(Image image,  int width, int height)
        {
            int resize_width = 0;
            int resize_height = 0;

            if ((double)image.Width / (double)image.Height > (double)width / (double)height)
            {
                resize_width = (int)Math.Ceiling(((double)height / (double)image.Height) * image.Width);
                resize_height = height;
                //image.Resize(999999, height + 1, true, true).Crop(1, 1);
                //image = ResizeImage(image, 999999, height);
            }
            else
            {
                resize_width = width;
                resize_height = (int)Math.Ceiling(((double)width / (double)image.Width) * image.Height); ;
                //image.Resize(width + 1, 999999, true, true).Crop(1, 1);
                //image = ResizeImage(image, width, 999999);
            }

            double crop_val_x = (double)(resize_width - width) / 2;
            double crop_val_y = (double)(resize_height - height) / 2;

            //image.Crop(0, (int)Math.Ceiling(crop_val), image.Height - height, (int)Math.Floor((double)(crop_val)));

            try
            {
                Image thumbNail = null;
                using (Image img = image)
                {
                    thumbNail = new Bitmap(resize_width+1, resize_height+1, img.PixelFormat);
                    Graphics g = Graphics.FromImage(thumbNail);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    Rectangle rect = new Rectangle(0, 0, resize_width+1, resize_height+1 );
                    g.DrawImage(img,rect);
                   //crop_val g.DrawImage(img, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);
                    //thumbNail = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                   // thumbNail.Save(resizedFile, format);
                }

                using (Image img = thumbNail)
                {
                    thumbNail = new Bitmap(width, height, img.PixelFormat);
                    Graphics g = Graphics.FromImage(thumbNail);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(img, new Rectangle(0, 0, width, height),(int)Math.Ceiling(crop_val_x)+ 1, (int)Math.Ceiling(crop_val_y)+1, width, height, GraphicsUnit.Pixel);
                    //thumbNail = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                    // thumbNail.Save(resizedFile, format);
                }
                return thumbNail;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool DeleteImage(string url,string width = null, string height = null)
        {
            try
            {
                string full_img_url  = "";
                if (width != null && height != null)
                {
                    full_img_url = Path.GetDirectoryName(url) + "/" + GenerateImageFileName(url, width, height);
                }
                else
                {
                    full_img_url = url;
                }
                if (File.Exists(full_img_url))
                    File.Delete(full_img_url);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetImagesDirectory()
        {
            return "/Images/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month + "/";
        }

        public static string GenerateImageFileName(string file_name, string width, string height)
        {
            string ret_path=  Path.GetFileNameWithoutExtension(file_name) + "-" + width + "x" + height + Path.GetExtension(file_name);
            return ret_path.Replace("\\", "/");
        }

        public static string GenerateImageFullPath(string image_path, string width, string height)
        {

            string ret_path =  Path.GetDirectoryName(image_path) + "/" + Path.GetFileNameWithoutExtension(image_path) + "-" + width + "x" + height + Path.GetExtension(image_path);

            return ret_path.Replace("\\", "/");
        }
        #endregion
    }
}

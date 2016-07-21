using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Common
{
    public class ImageHelper
    {


        #region GetPicThumbnail
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth"></param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>

        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            //int sWidth = iSource.Width;
            //int sHeight = iSource.Height;
            //if(sWidth>sHeight)
            //{
            //   dHeight=dWidth/sWidth*sHeight
            //}


            ImageFormat tFormat = iSource.RawFormat;


            int sW = 0, sH = 0;

            //按比例缩放

            Size tem_size = new Size(iSource.Width, iSource.Height);

            if (tem_size.Width >= tem_size.Height)
            {
                sW = dWidth;
                sH = (tem_size.Height / tem_size.Width) * dWidth;
            }
            else
            {
                sH = dHeight;
                sW = (tem_size.Width / tem_size.Height) * dHeight;
            }



            if (tem_size.Width > dHeight || tem_size.Width > dWidth) //将**改成c#中的或者操作符号
            {

                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {

                    sW = dWidth;

                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }

                else
                {

                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }

            else
            {

                sW = tem_size.Width;

                sH = tem_size.Height;

            }

            Bitmap ob = new Bitmap(dWidth, dHeight);

            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);

            g.CompositingQuality = CompositingQuality.HighQuality;

            g.SmoothingMode = SmoothingMode.HighQuality;

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量

            EncoderParameters ep = new EncoderParameters();

            long[] qy = new long[1];

            qy[0] = flag;//设置压缩的比例1-100

            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);

            ep.Param[0] = eParam;

            try
            {

                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();

                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)
                {

                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {

                        jpegICIinfo = arrayICI[x];

                        break;

                    }

                }

                if (jpegICIinfo != null)
                {

                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径

                }

                else
                {

                    ob.Save(dFile, tFormat);

                }

                return true;

            }

            catch
            {

                return false;

            }

            finally
            {

                iSource.Dispose();

                ob.Dispose();

            }



        }
        #endregion

        //按宽度压缩
        public static bool GetPicThumbnailWidth(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            //int sWidth = iSource.Width;
            //int sHeight = iSource.Height;
            //if(sWidth>sHeight)
            //{
            //   dHeight=dWidth/sWidth*sHeight
            //}


            ImageFormat tFormat = iSource.RawFormat;


            decimal sW = 0M, sH = 0M;

            //按比例缩放

            Size tem_size = new Size(iSource.Width, iSource.Height);

            if (tem_size.Width >= tem_size.Height)
            {
                sW = dWidth;
                sH = (tem_size.Height * dWidth) /( tem_size.Width);
            }
            else
            {
                sW = dWidth;
                sH = (tem_size.Height * dWidth) / (tem_size.Width);
            }
             
            Bitmap ob = new Bitmap(Convert.ToInt32(sW),Convert.ToInt32(sH));
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            //g.CompositingQuality = CompositingQuality.HighQuality;
            //g.SmoothingMode = SmoothingMode.HighQuality;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;


            g.CompositingQuality = CompositingQuality.HighSpeed;
           // g.SmoothingMode = SmoothingMode.None;
          //  g.InterpolationMode = InterpolationMode.Default;



           Rectangle rect= new Rectangle(0,0,Convert.ToInt32(sW), Convert.ToInt32(sH));
           g.DrawImage(iSource, rect, 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量

            EncoderParameters ep = new EncoderParameters();

            long[] qy = new long[1];

            qy[0] = flag;//设置压缩的比例1-100

            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);

            ep.Param[0] = eParam;

            try
            {

                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();

                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)
                {

                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {

                        jpegICIinfo = arrayICI[x];

                        break;

                    }

                }

                if (jpegICIinfo != null)
                {

                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径

                }

                else
                {

                    ob.Save(dFile, tFormat);

                }

                return true;

            }

            catch
            {

                return false;

            }

            finally
            {

                iSource.Dispose();

                ob.Dispose();

            }
 
        }
    }
}

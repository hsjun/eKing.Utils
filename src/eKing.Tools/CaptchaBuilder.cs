using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace eKing.Tools
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class CaptchaBuilder
    {
        #region 属性
        #region 验证码长度
        int length = 4;
        /// <summary>
        /// 验证码长度，默认值为4
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        #endregion

        #region 验证码字体大小
        int fontSize = 20;
        /// <summary>
        /// 验证码字体大小，默认值为20
        /// </summary>
        /// <remarks>为了显示扭曲效果，默认40像素，可以自行修改</remarks>
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion

        #region 边框间隙
        int padding = 1;
        /// <summary>
        /// 边框间隙，默认1像素
        /// </summary>
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 是否输出燥点
        bool chaos = true;
        /// <summary>
        /// 是否输出燥点，默认true（输出）
        /// </summary>
        public bool Chaos
        {
            get { return chaos; }
            set { chaos = value; }
        }
        #endregion

        #region 输出燥点的颜色
        Color chaosColor = Color.LightGray;
        /// <summary>
        /// 输出燥点的颜色，默认灰色
        /// </summary>
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 自定义背景色
        Color backgroundColor = Color.White;
        /// <summary>
        /// 背景色，默认白色
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义随机颜色数组
        Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        /// <summary>
        /// 随机颜色数组
        /// </summary>
        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }
        #endregion

        #region 自定义字体数组
        string[] fonts = { "Arial", "Georgia" };
        /// <summary>
        /// 字体数组，默认"Arial", "Georgia"
        /// </summary>
        public string[] Fonts
        {
            get { return fonts; }
            set { fonts = value; }
        }
        #endregion

        #region 自定义随机码字符串序列(使用逗号分隔)
        string codeSerial = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
        /// <summary>
        /// 随机码字符串序列(使用逗号分隔)，默认10个阿拉伯数字和26个大小写字母
		///去掉 l\I\1(数字)\o\O\0(数字)易混淆字符
        /// </summary>
        public string CodeSerial
        {
            get { return codeSerial; }
            set { codeSerial = value; }
        }
        #endregion
        #endregion

        #region 生成校验码图片
        /// <summary>
        /// 生成校验码图片
        /// </summary>
        /// <param name="captchaCode"></param>
        /// <returns></returns>
        public System.Drawing.Bitmap CreateImageCode(out string captchaCode)
        {
            string code = CreateVerifyCode();
            //返回实际码
            captchaCode = code;

            int fSize = this.fontSize;
            int fWidth = fSize + this.padding;
            int imageWidth = (int)(code.Length * fWidth) + 4 + this.padding * 2;
            int imageHeight = fSize * 2 + this.padding;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);
            Graphics g = Graphics.FromImage(image);
            g.Clear(this.backgroundColor);
            Random rand = new Random();

            //给背景添加随机生成的燥点
            if (this.chaos)
            {
                Pen pen = new Pen(this.chaosColor, 0);
                int c = this.length * 20;

                for (int i = 0; i < c; i++)
                {
                    int x = rand.Next(image.Width);
                    int y = rand.Next(image.Height);

                    g.DrawRectangle(pen, x, y, 1, 1);
                }
            }

            int left = 0, top = 0, top1 = 1, top2 = 1;

            int n1 = (imageHeight - this.fontSize - this.padding * 2);
            int n2 = n1 / 4;
            top1 = n2;
            top2 = n2 * 2;

            Font f;
            Brush b;
            int cindex, findex;

            //随机字体和颜色的验证码字符
            for (int i = 0; i < code.Length; i++)
            {
                cindex = rand.Next(this.colors.Length - 1);
                findex = rand.Next(this.fonts.Length - 1);

                f = new System.Drawing.Font(this.fonts[findex], fSize, System.Drawing.FontStyle.Bold);
                b = new System.Drawing.SolidBrush(this.colors[cindex]);

                if (i % 2 == 1)
                {
                    top = top2;
                }
                else
                {
                    top = top1;
                }

                left = i * fWidth;

                g.DrawString(code.Substring(i, 1), f, b, left, top);
            }

            //画一个边框 边框颜色为Color.Gainsboro
            //g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
            g.Dispose();

            //产生波形
            image = TwistImage(image, true, 3, 4);
/*
			MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            Response.ClearContent();
            Response.ContentType = "image/Png";
            Response.BinaryWrite(ms.ToArray());
            theGraphics.Dispose();
            theBitmap.Dispose();
            Response.End();
*/			

			
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ////将图像保存到指定的流
            //image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //return ms.GetBuffer();

            return image;
        }

        #region 产生波形滤镜效果
        private const double PI = 8.1415926535897932384626433832795;
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        private System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }
        #endregion
        #endregion

        #region 生成随机字符码
        /// <summary>
        /// 生成随机字符码
        /// </summary>
        /// <param name="codeLen">验证码长度，默认值4</param>
        /// <returns></returns>
        public string CreateVerifyCode()
        {
            string[] arr = this.codeSerial.Split(',');
            string code = "";
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));

            for (int i = 0; i < this.length; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                code += arr[randValue];
            }

            return code;
        }
        #endregion
    }
}


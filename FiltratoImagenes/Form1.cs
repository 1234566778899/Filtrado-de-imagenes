using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiltratoImagenes
{
    public partial class imgOriginal : Form
    {
        public imgOriginal()
        {
            InitializeComponent();
        }

        private void abrirImagenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Archivos de imagen (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp";
            openFileDialog1.Title = "Seleccionar imagen";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string imagePath = openFileDialog1.FileName;
                imgOriginal2.ImageLocation = imagePath;
                imgOriginal2.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
        private void ConvertirAEscaladeGrises(Bitmap imagenOriginal)
        {
            int promedio;
            Color pixelColor;
            int ancho = imagenOriginal.Width;
            int alto = imagenOriginal.Height;
            Bitmap imagenGrises = new Bitmap(ancho, alto);

            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {
                    pixelColor = imagenOriginal.GetPixel(x, y);
                    promedio = (int)((pixelColor.R * 0.3) + (pixelColor.G * 0.59) + (pixelColor.B * 0.11));
                    imagenGrises.SetPixel(x, y, Color.FromArgb(promedio, promedio, promedio));
                }
            }
            imgOriginal2.Image = imagenGrises;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var frmCarga = new frmCarga();
            frmCarga.Show();

            // Ejecutar la función en segundo plano
            await Task.Run(() => {
                ConvertirAEscaladeGrises((Bitmap)imgOriginal2.Image);
            });

            frmCarga.Close();
        }

        private async void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Media")
            {
                var frmCarga = new frmCarga();
                frmCarga.Show();

                // Ejecutar la función en segundo plano
                await Task.Run(() => {
                    imgResultado.Image = AplicarFiltroMedia((Bitmap)imgOriginal2.Image);
                });

                frmCarga.Close();
            }else if (comboBox1.Text == "Mediana")
            {
                var frmCarga = new frmCarga();
                frmCarga.Show();

                // Ejecutar la función en segundo plano
                await Task.Run(() => {
                    imgResultado.Image = AplicarFiltroMediana((Bitmap)imgOriginal2.Image);
                });

                frmCarga.Close();

            }
            else if (comboBox1.Text == "Laplaciano")
            {
                var frmCarga = new frmCarga();
                frmCarga.Show();

                // Ejecutar la función en segundo plano
                await Task.Run(() => {
                    imgResultado.Image = AplicarFiltroLaplaciano((Bitmap)imgOriginal2.Image);
                });

                frmCarga.Close();
            }
            else
            {
                var frmCarga = new frmCarga();
                frmCarga.Show();

                // Ejecutar la función en segundo plano
                await Task.Run(() => {
                    imgResultado.Image = AplicarFiltroSobel((Bitmap)imgOriginal2.Image);
                });

                frmCarga.Close();
            }
        }

        private void imgOriginal_Click(object sender, EventArgs e)
        {

        }

        public Bitmap AplicarFiltroSobel(Bitmap imagen)
        {
            //Máscaras para detección de bordes en dirección X y Y
            int[,] mascaraX = new int[,] {{-1,0,1},
                                    {-2,0,2},
                                    {-1,0,1}};

            int[,] mascaraY = new int[,] {{-1,-2,-1},
                                    {0,0,0},
                                    {1,2,1}};

            int tamanoMascara = 3;
            Bitmap imagenResultado = new Bitmap(imagen.Width, imagen.Height);

            for (int x = 1; x < imagen.Width - 1; x++)
            {
                for (int y = 1; y < imagen.Height - 1; y++)
                {
                    int valorNuevoPixelX = 0;
                    int valorNuevoPixelY = 0;

                    //Obtenemos los valores de los pixeles que forman la máscara X
                    for (int i = -tamanoMascara / 2; i <= tamanoMascara / 2; i++)
                    {
                        for (int j = -tamanoMascara / 2; j <= tamanoMascara / 2; j++)
                        {
                            int pixelX = x + i;
                            int pixelY = y + j;

                            int valorMascaraX = mascaraX[i + tamanoMascara / 2, j + tamanoMascara / 2];

                            if (pixelX >= 0 && pixelX < imagen.Width && pixelY >= 0 && pixelY < imagen.Height)
                            {
                                int valorPixelGrises = imagen.GetPixel(pixelX, pixelY).R;

                                valorNuevoPixelX += valorPixelGrises * valorMascaraX;
                            }
                        }
                    }

                    //Obtenemos los valores de los pixeles que forman la máscara Y
                    for (int i = -tamanoMascara / 2; i <= tamanoMascara / 2; i++)
                    {
                        for (int j = -tamanoMascara / 2; j <= tamanoMascara / 2; j++)
                        {
                            int pixelX = x + i;
                            int pixelY = y + j;

                            int valorMascaraY = mascaraY[i + tamanoMascara / 2, j + tamanoMascara / 2];

                            if (pixelX >= 0 && pixelX < imagen.Width && pixelY >= 0 && pixelY < imagen.Height)
                            {
                                int valorPixelGrises = imagen.GetPixel(pixelX, pixelY).R;

                                valorNuevoPixelY += valorPixelGrises * valorMascaraY;
                            }
                        }
                    }

                    //Calculamos el valor del nuevo pixel
                    double valorNuevoPixel = Math.Sqrt(valorNuevoPixelX * valorNuevoPixelX + valorNuevoPixelY * valorNuevoPixelY);

                    //Ajustamos el rango de valores
                    if (valorNuevoPixel < 0)
                    {
                        valorNuevoPixel = 0;
                    }
                    else if (valorNuevoPixel > 255)
                    {
                        valorNuevoPixel = 255;
                    }

                    //Asignamos el valor al pixel actual en la imagen resultado
                    imagenResultado.SetPixel(x, y, Color.FromArgb((int)valorNuevoPixel, (int)valorNuevoPixel, (int)valorNuevoPixel));
                }
            }
            return imagenResultado;
        }
        public Bitmap AplicarFiltroLaplaciano(Bitmap imagen)
        {
            int[,] mascara = new int[,]{{-1,-1,-1},
                                  {-1, 8,-1},
                                  {-1,-1,-1}};

            int tamanoMascara = 3;
            Bitmap imagenResultado = new Bitmap(imagen.Width, imagen.Height);

            for (int x = 1; x < imagen.Width - 1; x++)
            {
                for (int y = 1; y < imagen.Height - 1; y++)
                {
                    int valorNuevoPixel = 0;

                    //Obtenemos los valores de los pixeles que forman la máscara
                    for (int i = -tamanoMascara / 2; i <= tamanoMascara / 2; i++)
                    {
                        for (int j = -tamanoMascara / 2; j <= tamanoMascara / 2; j++)
                        {
                            int pixelX = x + i;
                            int pixelY = y + j;

                            int valorMascara = mascara[i + tamanoMascara / 2, j + tamanoMascara / 2];

                            if (pixelX >= 0 && pixelX < imagen.Width && pixelY >= 0 && pixelY < imagen.Height)
                            {
                                int valorPixelGrises = imagen.GetPixel(pixelX, pixelY).R;

                                valorNuevoPixel += valorPixelGrises * valorMascara;
                            }
                        }
                    }

                    //Ajustamos el rango de valores
                    if (valorNuevoPixel < 0)
                    {
                        valorNuevoPixel = 0;
                    }
                    else if (valorNuevoPixel > 255)
                    {
                        valorNuevoPixel = 255;
                    }

                    //Asignamos el valor al pixel actual en la imagen resultado
                    imagenResultado.SetPixel(x, y, Color.FromArgb(valorNuevoPixel, valorNuevoPixel, valorNuevoPixel));
                }
            }
            return imagenResultado;
        }
        public Bitmap AplicarFiltroMediana(Bitmap imagen)
        {
            int tamanoMascara = 3;
            Bitmap imagenResultado = new Bitmap(imagen.Width, imagen.Height);

            for (int x = 0; x < imagen.Width; x++)
            {
                for (int y = 0; y < imagen.Height; y++)
                {
                    List<int> valoresPixel = new List<int>();

                    //Obtenemos los valores de los pixeles que forman la máscara
                    for (int i = -tamanoMascara / 2; i <= tamanoMascara / 2; i++)
                    {
                        for (int j = -tamanoMascara / 2; j <= tamanoMascara / 2; j++)
                        {
                            int pixelX = x + i;
                            int pixelY = y + j;

                            if (pixelX >= 0 && pixelX < imagen.Width && pixelY >= 0 && pixelY < imagen.Height)
                            {
                                valoresPixel.Add(imagen.GetPixel(pixelX, pixelY).R);
                            }
                        }
                    }

                    //Ordenar los valores de menor a mayor
                    valoresPixel.Sort();

                    //Obtener el valor mediano
                    int valorMediano = valoresPixel[valoresPixel.Count / 2];

                    //Asignamos el valor mediano al pixel actual en la imagen resultado
                    imagenResultado.SetPixel(x, y, Color.FromArgb(valorMediano, valorMediano, valorMediano));
                }
            }
            return imagenResultado;
        }
        public Bitmap AplicarFiltroMedia(Bitmap bmp)
        {
            if (bmp == null)
                throw new ArgumentNullException("bmp");

            Bitmap filteredBmp = new Bitmap(bmp.Width, bmp.Height);

            int filterSize = 3; // Tamaño del filtro
            int[] pixels = new int[filterSize * filterSize];

            // Iteramos sobre todos los píxeles de la imagen
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    // Recorremos los píxeles alrededor del actual para aplicar el filtro de media
                    int index = 0;
                    for (int j = -filterSize / 2; j <= filterSize / 2; j++)
                    {
                        for (int i = -filterSize / 2; i <= filterSize / 2; i++)
                        {
                            int px = x + i;
                            int py = y + j;

                            if (px < 0 || px >= bmp.Width || py < 0 || py >= bmp.Height)
                                continue;

                            // Obtenemos el valor del pixel en escala de grises
                            Color c = bmp.GetPixel(px, py);
                            int grayValue = (int)(0.2989 * c.R + 0.5870 * c.G + 0.1140 * c.B);

                            pixels[index++] = grayValue;
                        }
                    }

                    // Ordenamos los valores de los píxeles y tomamos el valor central
                    Array.Sort(pixels);
                    int medianValue = pixels[(filterSize * filterSize) / 2];

                    // Aplicamos el valor de la mediana al pixel actual
                    filteredBmp.SetPixel(x, y, Color.FromArgb(medianValue, medianValue, medianValue));
                }
            }

            return filteredBmp;
        }
    }
}

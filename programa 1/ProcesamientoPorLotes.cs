using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Drawing.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace programa_1
{
    public partial class ProcesamientoPorLotes : Form
    {
        //Se crea el reloj global
        Stopwatch oSW = new Stopwatch();
        
        //Arreglo con diferentes nombres
        string[] listNombres =
{
             "Jorge", "Ivan", "Edwin","Paco","Hector",
             "Hugo","Eduardo","Luis","Pedro","Maria","Mariana","Patricia","Liz",
             "Fernanda","Alondra","Angela"
            };

        //Arreglo con diferentes operadores
        char[] listOperadores =
        {
                '+', '-', '*', '/'
            };

        //Generacion de los elementos aleatorios
        Random aleatorio = new Random();
        bool interrupted = false; //booleano para detectar si se ha hecho una interrupcion
        int acumulador; //tal cual, un acumulador
        int contRes = 0; //contador para los restantes
        int Nloot = -1; //Nloot sera el contador de lotes que incrementaran cada 5 elementos o los ultimos elementos
        int tm = -1; //Para las posiciones
        bool error = false; //booleano para detectar si se ha hecho un error
        List<Programador> auxdatos = new List<Programador>(); //Lista secundaria que guardara elementos de 5 en 5

        public ProcesamientoPorLotes()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        //Funcion creada por alguien en stackoverflow, 
        //Otorga la posibilidad de pausar el prorgrama por unos segundos
        //Con ponel wait(x) en donde se necesite
        public void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        public void Cronometro(TextBox textbox, bool triger)
        {
            int y = 0;
            do
            {
                textbox.Text += y;
                wait(1000);
                y++;
                textbox.Text = "";
            } while (triger == true);
        }

        //Esta funcion sirve para la ejecucion y segundos en tiempo real
        public int EjecutarYTemporizador(TextBox textbox, Programador programador, int i)
        { 
            int x = programador.tiempo / 1000;
            int TMrestante = programador.auxTM; // Tomara el tiempo restante si es interrumpido
            do
            {
                if (programador.state == false) //Dependiendo del estado del programador si es interrumpido o no
                {                               //Mostrara una forma diferente de imprimirse en Ejecucion
                    
                        textbox.Text += programador.id + ". ";
                        textbox.Text += programador.nombre + "\r\n";
                        textbox.Text += programador.num1 + " ";
                        textbox.Text += programador.simbolo + " ";
                        textbox.Text += programador.num2 + " " + "\r\nTME: ";
                        textbox.Text += x + "\r\n";
                        wait(1000);
                        x--;
                        contRes = x + 1;                        
                        textbox.Text = "";                       
                }

                if (interrupted == true) //Los cambios de posicion en caso de que se haya interrumpido
                {
                    var position = auxdatos[i - 1]; //Guardamos la posicion actual
                    auxdatos[i - 1].state = true; //Hacemos que el estado del programador pase a interrumpido
                    auxdatos[i - 1].auxTM = contRes; //Se guarda el tiempo que le falto para terminar

                    auxdatos.Add(position); //Agregamos el elemento al final de la lista
                    

                    interrupted = false; //Desactivamos el booleano global para activar la interrupcion
                    acumulador--; //Disminuimos el acumulador para que los numeros esten bien
                    tm++; //La posicion no cambiara con esto

                    return 1; //Retornara 1 en la funcion si fue interrumpido
                    
                }

                if (error == true) //Si el proceso tuvo un error, terminara abruptamente
                {
                    x = 0;
                }


                if (programador.state == true) //La forma de imprimir al programador si este fue interrumpido
                {
                  
                    textbox.Text += programador.id + ". ";
                    textbox.Text += programador.nombre + "\r\n";
                    textbox.Text += programador.num1 + " ";
                    textbox.Text += programador.simbolo + " ";
                    textbox.Text += programador.num2 + " " + "\r\nTME restante: ";
                    textbox.Text += TMrestante + "\r\n";
                    textbox.Text += "Tiempo antes de la interrupcion: " + programador.tiempo / 1000 + "\r\n";
                    wait(1000);
                    TMrestante--;
                    x = TMrestante;
                    textbox.Text = "";
                   
                }
                
            } while (x != 0);

            return 0; //Si el proceso paso sin problemas, retorna 0
           
        }

        

        //Se capturan los datos entrantes con dos formas para ser expresada
        public void Printprogramertotextbox(TextBox textbox, Programador programador, int moredetails)
        {


            if (moredetails == 1)
            {
                textbox.Text += programador.id + ". ";
                textbox.Text += programador.nombre + "\r\n";
                textbox.Text += programador.num1 + " ";
                textbox.Text += programador.simbolo + " ";
                textbox.Text += programador.num2 + " " + "\r\nTME: ";
                textbox.Text += programador.tiempo / 1000 + "\r\n";
            }
            else if (moredetails == 2)
            {
                if (error == false) //Forma de imprimir al programador si este paso bien
                {
                    textbox.Text += programador.id + ". ";
                    textbox.Text += programador.nombre + "\r\n";
                    textbox.Text += programador.num1 + " ";
                    textbox.Text += programador.simbolo + " ";
                    textbox.Text += programador.num2 + " " + " = ";
                    textbox.Text += programador.res + "\r\n" + "\r\n";
                }

                if (error == true) //o paso con un error 
                {
                    textbox.Text += programador.id + ". ";
                    textbox.Text += programador.nombre + "\r\n";
                    textbox.Text += "ERROR" + "\r\n" + "\r\n";
                    error = false;
                }

            }
        }
        //Metodo para identificar la operacion y resolverla
        public float RealizarOperacion(char operacion, int num1, int num2)
        {
            float res = 0;

            if (operacion == '+')
            {
                res = num1 + num2;
            }
            else if (operacion == '*')
            {
                res = num1 * num2;
            }
            else if (operacion == '-')
            {
                res = num1 - num2;
            }
            else if (operacion == '/')
            {
                res = num1 / num2;
            }
            return res;
        }

        //clase programador donde se guardaran los datos en variables
        public class Programador
        {
            public int id;
            public string nombre;
            public int num1, num2, tiempo;
            public char simbolo;
            public float res;
            public bool state;
            public int auxTM;

            public Programador(int nid, int nnum1, int nnum2,int ntiempo, char nsimbolo, string nnombre, float nres, bool nstate, int nauxTM)
            {
                id = nid;
                nombre = nnombre;
                num1 = nnum1;
                num2 = nnum2;
                tiempo = ntiempo;
                simbolo = nsimbolo;
                res = nres;
                state= nstate;
                auxTM = nauxTM;


            }
        }

        //Las funciones que comienzan con el boton de "Generar"
        private async void ButtonGen_Click(object sender, EventArgs e) 
        {
            
            oSW.Start(); //Con este comando comienza el temporizador
            timer1.Enabled = true; //Con este otro activamos la herramienta Timer
            //Cronometro(Seg, true);
            //Con esto agregamos el numero de programadores que se crearan
            int elementos = Convert.ToInt32(NumProc.Text);
            
            //variables para el ciclo y un auxiliar para los lotes
            int a;
            int numlot1 = elementos / 5; //Se capturan el numero de lotes que habra
            int auxesp = 0; //Contador auxiliar de los elementos
            int auxres = 0; //Auxiliar para el decremento del numero de lotes


            //lista donde se agregaran los datos de los programadores
            List<Programador> datos = new List<Programador>();
            

            //Se limpia la pantalla en caso de volver a tener el programa con datos
            EnEspera.Text = "";
            Terminados.Text = "";
            //Se crea un archivo de texto nuevo en caso de tener uno ya existente
            //para que cada que se generen nuevos datos los viejos desaparezcan.
            Ghost.Text = "";// Limpiamos ghost para que no se acumulen los datos
            //Se crea un nuevo archivo limpio de Datos.txt
            using (FileStream fs = new FileStream("Datos.txt", FileMode.Create))
            {
                using (StreamWriter data = new StreamWriter(fs))
                {
                    await data.WriteAsync(Ghost.Text);
                }
            }

            //Comienzan los cliclos
            //El primero es para agregar los datos a los programadores
            for (a = 1; a <= elementos; a++)
            {
                int id = a;
                string nombre = listNombres[aleatorio.Next(listNombres.Length)];
                int num1 = aleatorio.Next(50, 101);
                int num2 = aleatorio.Next(50);
                char simbol = listOperadores[aleatorio.Next(listOperadores.Length)];
                int tiempo = aleatorio.Next(5000, 13000);
                float res;
                bool state = false; //Estado del programador, se volvera true si fue interrumpido
                int auxTM = 0; //valor que guardara el tiempo restante en caso de ser interrumpido

                //Aqui se agregan los datos recibidos para el metodo realizarOperacion
                res = RealizarOperacion(simbol, num1, num2);
                //Aqui se guardan los datos que se capturaron en las variables 
                Programador nprogramador = new Programador(id, num1, num2, tiempo, simbol, nombre, res, state, auxTM);
                datos.Add(nprogramador);// Se guarda en la lista datos
                auxesp++;//El contador auxiliar incrementa hasta llegar al maximo de elementos agregados
                auxres = numlot1 + 1; //se le da el valor de numero de lotes existentes a auxres
            }
            
            
            
            for (int j = 1; j <= datos.Count(); j++) //Ciclo general para todo el procesamiento
            {
                acumulador = 0; //Contara cuantos programadores hay en la lista
                Nloot++; //Se incrementa la cantidad de lootes en 1
                auxres--; //Se decrementa el numero maximo de lootes para la textbox que muestra el numero restante de lotes
                tm = -1; //Numero de programadores en espera, empieza con -1 por el programador que pasa en ejecucion
                auxdatos.Clear(); //Se limpiara la lista secundaria
                for (int i = 0; i < 5; i++) //Ciclo para agregar 5 elementos de la lista principal a la secundaria
                {
                    try
                    {
                        tm++;
                        auxdatos.Add(datos[0]); // el primer elemento de la lista principal sera agregados a la lista secundaria
                        datos.RemoveAt(0); // Cada elemento que sea agregado sera eliminado y la lista se recorrera
                    }
                    catch(Exception)
                    {
                        tm--; //En caso de que falten elementos, el contador de programadores por lotes se ajustara al numero faltante
                    }
                }



                for (int i = 1; i <= auxdatos.Count(); i++) //Ciclo para el procesamiento de los 5 elementos en la lista secundaria
                {

                    bool imprimirlote = false; //Iniciamos una variable en falso
                    if (i != auxdatos.Count()+1) //COndicinal para imprimir de 1 elemento a la vez
                    {
                        acumulador++; //El contador acumulador incrementa
                        try //El try es solo para evitar un crasheo del programa
                        {
                            Printprogramertotextbox(EnEspera, auxdatos[i], 1); //imprime los elementos en espera en su textbox
                        }catch (Exception)
                        {

                        }
                        
                        
                        
                        if (acumulador == 5) //Si el acumulador alcanzo 5 elementos...
                        {
                            
                            try
                            {
                                Printprogramertotextbox(EnEspera, datos[0], 1); //Mostrara los datos que no esten dentro de la lista secundaria
                            }                                                   //Para evitar vacios hasta que entre el nuevo lote
                            catch(Exception)
                            {

                            }
                        }


                        if (i % 6 == 0 && i != 0) 
                        {
                            try
                            {
                                if (auxdatos[i].state == false) //Si el estado del programador no es interrumpido...
                                {
                                    imprimirlote = true; //El bool se vuelve true
                                    tm = 5; //El contador de numero de procesos se vuelve en 5 
                                }
                            }
                            catch (Exception)
                            {

                            }
 
                            
                            
                            
                           
                            if (Nloot == numlot1)
                            {
                                tm = elementos % 5; //Cuando Nloot alcanza al numero maximo de lotes, se sacan los ultimos elementos 
                            }

                        }
                        
                        textBox2.Text = ""; //Se limpia la textbox del maximo de lootes para el auxiliar del decremento
                        textBox2.Text += auxres; //Se ingresa el nuevo valor 
                        EnEspera.Text += "\r\n" + "\r\n" + "Procesos en espera = " + (tm); //Se imprime el numero de procesos pendientes por lote
                        
                        tm--; //Esto decrementa el numero de procesos cuando pasa a ejecucion
                    }

                    //Agrega la informacion al segundo textbox "Ejecucion"
                    int finalizado = EjecutarYTemporizador(Ejecutar, auxdatos[i - 1],i); //Se ingresan datos en una posicion diferente para simular que se mueven los datos
                    //Dependiendo el valor en el return (1 ó 0) de este proceso, se guardara en finalizado

                        if (imprimirlote || i == 1) //Si se cumple la condicional, se imprimira un numero de lote donde corresponde
                        {
                            Ghost.Text += "Lote: " + (Nloot + 1) + "\r\n"; //Se estampa el numero de lote para Ghost
                        }
                        Printprogramertotextbox(Ghost, auxdatos[i - 1], 1); //Se agregan datos en una textbox fantasma para acomodar Datos.txt

                        if (imprimirlote || i == 1) //Si se cumple la condicional, se imprimira un numero de lote donde corresponde
                        {
                            Terminados.Text += "Lote: " + (Nloot + 1) + "\r\n";//Se estampa el numero de lote para terminados

                        }
          
                        if(finalizado == 0) //pasaran los elementos que no hayan recibido un 1 (interrumpido) mientras se procesaban
                        {
                        Printprogramertotextbox(Terminados, auxdatos[i - 1], 2);//Se agregan datos para la textbox terminados
                        EnEspera.Text = ""; //Se limpiaran los datos anteriores que estaban en la textbox EnEspera
                        //Cuando les toque pasar nuevamente, pasaran a imprimirse sin problemas
                    }
                    else
                    {
                        EnEspera.Text = "";
                    }

                        //Los datos que han agregado en Ghost comienzan a ser agregados al .txt
                        using (FileStream fs = new FileStream("Datos.txt", FileMode.Create))
                        {
                            using (StreamWriter data = new StreamWriter(fs))
                            {
                                await data.WriteAsync(Ghost.Text); //Toma los datos que estan en la textbox y los pone en el .txt
                            }
                        }
                    
                }
            }
            
        }
 
        private void NumProc_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Con estos comandos configuramos el Timer para que funcione con minutos, segundos y milisegundos
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)oSW.ElapsedMilliseconds);
            Seg.Text = ts.Seconds.ToString(); //Asignamos el textbox para el contador de segundos
        }

        private void Ejecutar_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
                //Cuando se presione el boton resultado, se generara un documento .txt llamado resultados
                //En ese documento se guardaran los programadores junto con sus operaciones resueltas
                using (FileStream fs = new FileStream("Resultados.txt", FileMode.Create))
                {
                    using (StreamWriter data = new StreamWriter(fs))
                    {
                        data.WriteAsync(Terminados.Text);
                    }
                }           
        }

        private void Seg_TextChanged(object sender, EventArgs e)
        {

        }

        private void Interrupcion_Click(object sender, EventArgs e)
        {
            interrupted = true; //Si se presiona el boton, activara la secuencia para interrumpir un proceso con el valor global
          
        }

        private void Error_Click(object sender, EventArgs e)
        {
            error = true; //Si se presiona el boton, activiara la secuencia para dar error a un proceso con el valor global
        }
    }
}

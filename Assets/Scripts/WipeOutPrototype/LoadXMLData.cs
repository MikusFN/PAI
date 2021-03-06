using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;
//using Assets.MenuSettings.Scripts;
using System;

namespace Assets.GeneralScripts
{
    public class LoadXMLData : MonoBehaviour
    {
        /*How the Xml file is presented:
          <?xml version="1.0" encoding="utf-8"?>
          <ListWiGateWay Peso="85">
            <WiGateWay>
                <FE>0</FE>              -> Frente Esquerdo
                 <FD>0,0</FD>           -> Frente Direito
                 <TE>0</TE>             -> Trás Esquerdo
                 <TD>0</TD>             -> Trás Direito
            </WiGateWay>
          </ListWiGateWay>          
         */

        [Range(0.1f, 1.0f)] public float pesoValor;
        private static LoadXMLData _instance;
        public static LoadXMLData Instance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            Debug.Log("LoadXMLData Instance Not Initialized!!!");
            return null;
        }

        private static string PATH = "C:/Sensing Future Technologies/wigateway.xml";
        //private static string PATH = "C:/Users/tayna/Desktop/wigateway.xml";

        private float direita, esquerda, frente, tras, x = 0.0f, y = 0.0f, somaValores;
        public float _fe, _fd, _te, _td;
        public float _weight, amp;
        private Vector2 _virtualDirection = Vector2.zero;

        private XmlDocument _xmlDoc;
        //private StreamReader _reader;

        //Data Writer
        //private string _writerPath = "Assets/Resources/SessionStats.txt";
        //StreamWriter dataWriter;

        EventWaitHandle _waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");

        private void Awake()
        {
            //if (SetUp.Instance() != null)
            //{
            //    pesoValor = SetUp.Instance().Balance;
            //}
            //else
            //{
            //    pesoValor = 0.1f;
            //}

            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
            _xmlDoc = new XmlDocument();

            _fe = 0;
            _fd = 0;
            _td = 0;
            _te = 0;

            //dataWriter = new StreamWriter(_writerPath, true);
            //dataWriter.WriteLine(DateTime.Now);
                //LoadData();

        }

        public void Update()
        {
            //LoadData();
        }

        public void LoadData()
        {
            //wait thread
            _waitHandle.WaitOne();
            //read xml
            _xmlDoc.Load(PATH);
            //Debug.Log("XML DATA LOADED");

            XmlNodeList xmlNodeList = _xmlDoc.GetElementsByTagName("WiGateWay");

            if (xmlNodeList == null)
            {
                Debug.Log("xmlNodeList is null!!");
            }

            foreach (XmlNode node in xmlNodeList)
            {
                //Debug.Log("xmlNodeList NOT null!!");
                //Debug.Log("node.name -> " + node.Name);
                XmlNodeList childNodes = node.ChildNodes;

                _weight = float.Parse(_xmlDoc.SelectSingleNode("/ListWiGateWay/@Peso").Value);

                foreach (XmlNode child in childNodes)
                {
                    //Debug.Log("child.name -> " + child.Name);

                    switch (child.Name)
                    {
                        case "FE":

                            _fe = float.Parse(child.InnerText);
                            //Debug.Log("FE -> " + _fe + " x = " + _virtualDirection.x);
                            //dataWriter.WriteLine("FE");

                            break;

                        case "FD":

                            _fd = float.Parse(child.InnerText);
                            //Debug.Log("FD -> " + _fd + " x = " + _virtualDirection.x);
                            //dataWriter.WriteLine("FD");
                            break;

                        case "TE":

                            _te = float.Parse(child.InnerText);
                            //Debug.Log("TE -> " + _te + " x = " + _virtualDirection.x);
                            //dataWriter.WriteLine("TE");
                            break;

                        case "TD":

                            _td = float.Parse(child.InnerText);
                            //Debug.Log("TD -> " + _td + " x = " + _virtualDirection.x);
                            //dataWriter.WriteLine("TD");
                            break;
                    }

                }
                //   dataWriter.Close();

                Direita = (_td + _fd);
                Esquerda = (_fe + _te);
                Frente = (_fd + _fe);
                Tras = (_td + _te);

                //Direita = Direita *00000000000000.1f;
                //Esquerda = Esquerda * 00000000000000.1f;
                //Frente = Frente  * 00000000000000.1f;
                //Tras = Tras * 00000000000000.1f;


                somaValores = Direita + Esquerda + Frente + Tras;
                //Debug.Log("Direita -> " + Direita);
                //Debug.Log("Esquerda -> " + Esquerda);
                //Debug.Log("Frente -> " + Frente);
                //Debug.Log("Tras -> " + Tras);


                amp = Mathf.Sqrt(Mathf.Pow(Direita - Esquerda, 2) + Mathf.Pow(Frente - Tras, 2));
                //Debug.Log("amp: " + amp);
                x = pesoValor * x + (1 - pesoValor) * (Direita - Esquerda);
                y = pesoValor * y + (1 - pesoValor) * (Frente - Tras);

                if (amp < 25)
                {
                    x = 0.0f;
                    y = 0.0f;
                }
                else if (amp < 75)
                {
                    //Debug.Log("entrou" + amp);
                    x = 0.1f * (pesoValor * x + (1 - pesoValor) * (Direita - Esquerda));
                    y = 0.1f * (pesoValor * y + (1 - pesoValor) * (Frente - Tras));
                }
                else if (amp < 100)
                {
                    //Debug.Log("entrou" + amp);
                    x = (pesoValor * x + (1 - pesoValor) * (Direita - Esquerda));
                    y = (pesoValor * y + (1 - pesoValor) * (Frente - Tras));
                }
                else if (amp < 125)
                {
                    //Debug.Log("entrou" + amp);
                    x = 0.1f * (pesoValor * x + (1 - pesoValor) * (Direita - Esquerda));
                    y = 0.1f * (pesoValor * y + (1 - pesoValor) * (Frente - Tras));
                }
                else if (amp < 150)
                {
                    //Debug.Log("entrou" + amp);
                    x = 0.001f * (0.1f * (pesoValor * x + (1 - pesoValor) * (Direita - Esquerda)));
                    y = 0.001f * (0.1f * (pesoValor * y + (1 - pesoValor) * (Frente - Tras)));
                }


                _virtualDirection = new Vector2(x, y);
                //_virtualDirection = new Vector2((_td + _fd) - (_fe + _te), 0.0f);
                _waitHandle.Set();
            }
        }

        /// <summary>
        /// Returns the vector2 responsible for the direction given by the platform input 
        /// </summary>
        public Vector2 GetDirection
        {
            get { return _virtualDirection; }
        }

        public float GetWeight
        {
            get { return _weight; }
        }

        public float SomaValores
        {
            get
            {
                return somaValores;
            }
        }

        public float Direita
        {
            get
            {
                return direita;
            }

            set
            {
                direita = value;
            }
        }

        public float Esquerda
        {
            get
            {
                return esquerda;
            }

            set
            {
                esquerda = value;
            }
        }

        public float Frente
        {
            get
            {
                return frente;
            }

            set
            {
                frente = value;
            }
        }

        public float Tras
        {
            get
            {
                return tras;
            }

            set
            {
                tras = value;
            }
        }
    }
}
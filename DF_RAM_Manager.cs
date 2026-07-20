using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace RamManager
{
    static class Program
    {
        // Importa a API nativa do Windows para liberar a memória
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        static NotifyIcon trayIcon;
        static System.Timers.Timer timer;
        static MenuItem[] timerItems; // Array para controlar visualmente qual timer está marcado

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Adiciona o programa para iniciar junto com o Windows automaticamente
            SetStartup();

            // Configura o menu do botão direito
            ContextMenu menu = new ContextMenu();
            
            // Cria o submenu de configuração do Timer
            MenuItem timerMenu = new MenuItem("Set Timer");
            timerItems = new MenuItem[] {
                new MenuItem("5 min", Timer5_Click) { Checked = true },
                new MenuItem("10 min", Timer10_Click),
                new MenuItem("15 min", Timer15_Click),
                new MenuItem("30 min", Timer30_Click),
                new MenuItem("60 min", Timer60_Click)
            };
            timerMenu.MenuItems.AddRange(timerItems);

            menu.MenuItems.Add(timerMenu);
            menu.MenuItems.Add("-"); // Separador visual
            menu.MenuItems.Add("Free Memory Now", FreeMemoryNow_Click);
            menu.MenuItems.Add("Exit", Exit_Click);

            // Configura o ícone na bandeja do sistema
            trayIcon = new NotifyIcon();
            trayIcon.Text = "RAM Manager - Standby";
            
            try 
            { 
                // Lê o ícone embutido no próprio executável durante a compilação (Ultra Portátil)
                trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); 
            } 
            catch 
            { 
                // Fallback extremo: Extrai o ícone nativo do Gerenciador de Tarefas do Windows (taskmgr.exe)
                string taskMgrPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "taskmgr.exe");
                trayIcon.Icon = Icon.ExtractAssociatedIcon(taskMgrPath);
            }

            trayIcon.ContextMenu = menu;
            trayIcon.Visible = true;

            // Configura o Timer (Padrão inicial: 30 minutos = 1800000 ms)
            timer = new System.Timers.Timer(1800000); 
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            // Executa a aplicação silenciosamente (sem janela principal)
            Application.Run();
        }

        // Função que grava o executável na inicialização do Windows
        static void SetStartup()
        {
            try
            {
                // Como o programa exige Administrador, o Registro do Windows o bloqueia na inicialização.
                // A solução é criar uma tarefa invisível no Agendador de Tarefas com privilégios máximos.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "schtasks.exe";
                // Cria a tarefa executando no Logon do usuário (/sc onlogon) com privilégios de Admin (/rl highest). 
                // O /f força a atualização do caminho caso você mova o .exe de pasta.
                startInfo.Arguments = "/create /tn \"RamManager\" /tr \"\\\"" + Application.ExecutablePath + "\\\"\" /sc onlogon /rl highest /f";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
            catch 
            { 
                // Ignora silenciosamente erros caso não consiga criar a tarefa
            }
        }

        // Eventos de clique para cada opção de tempo
        static void Timer5_Click(object sender, EventArgs e) { ChangeTimer(5, (MenuItem)sender); }
        static void Timer10_Click(object sender, EventArgs e) { ChangeTimer(10, (MenuItem)sender); }
        static void Timer15_Click(object sender, EventArgs e) { ChangeTimer(15, (MenuItem)sender); }
        static void Timer30_Click(object sender, EventArgs e) { ChangeTimer(30, (MenuItem)sender); }
        static void Timer60_Click(object sender, EventArgs e) { ChangeTimer(60, (MenuItem)sender); }

        // Atualiza a velocidade do relógio e marca (check) a opção clicada
        static void ChangeTimer(int minutes, MenuItem clickedItem)
        {
            // Desmarca todas as opções e marca apenas a que foi selecionada
            foreach (MenuItem item in timerItems) item.Checked = false;
            clickedItem.Checked = true;

            timer.Stop();
            timer.Interval = minutes * 60 * 1000; // Converte minutos para milissegundos
            timer.Start();
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ClearMemory();
        }

        static void FreeMemoryNow_Click(object sender, EventArgs e)
        {
            // Para o timer, executa a limpeza e reinicia o timer do zero
            timer.Stop();
            ClearMemory();
            timer.Start();
        }

        static void Exit_Click(object sender, EventArgs e)
        {
            // Oculta o ícone e encerra o processo completamente
            trayIcon.Visible = false;
            Application.Exit();
        }

        static void ClearMemory()
        {
            // Atualiza o texto do ícone para dar feedback visual
            trayIcon.Text = "RAM Manager - Cleaning...";

            // Itera por absolutamente todos os processos rodando no sistema
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    // Tenta forçar o processo a liberar a RAM não utilizada
                    // Funciona nos processos do Windows se rodar como Administrador
                    EmptyWorkingSet(process.Handle);
                }
                catch (Exception)
                {
                    // Ignora silenciosamente processos protegidos pelo kernel
                    // (ex: System, Idle) para evitar crashes e garantir estabilidade
                }
            }

            trayIcon.Text = "RAM Manager - Standby";
        }
    }
}

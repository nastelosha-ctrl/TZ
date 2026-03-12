using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace TZ
{
    /// <summary>
    /// Класс для отслеживания бездействия пользователя
    /// </summary>
    public static class InactivityTimer
    {
        private static Timer _timer;
        private static int _timeoutSeconds;
        private static Form _currentForm;
        private static Action _onInactivity;
        private static Timer _countdownTimer;
        private static int _countdownSeconds = 10;
        private static Form _warningForm;
        private static bool _isWarningShowing = false; // Флаг, показывается ли предупреждение

        /// <summary>
        /// Инициализация таймера бездействия
        /// </summary>
        public static void Initialize(Form form, Action onInactivity)
        {
            _currentForm = form;
            _onInactivity = onInactivity;

            // Читаем настройку из App.config
            string timeoutStr = ConfigurationManager.AppSettings["InactivityTimeoutSeconds"];
            if (!int.TryParse(timeoutStr, out _timeoutSeconds) || _timeoutSeconds <= 0)
            {
                _timeoutSeconds = 30;
            }

            // Создаем таймер
            _timer = new Timer();
            _timer.Interval = _timeoutSeconds * 1000;
            _timer.Tick += Timer_Tick;

            // Подписываемся на события активности
            SubscribeToActivityEvents(form);

            // Запускаем таймер
            ResetTimer();
        }

        private static void SubscribeToActivityEvents(Control control)
        {
            // НЕ СБРАСЫВАЕМ ТАЙМЕР, ЕСЛИ ПОКАЗЫВАЕТСЯ ПРЕДУПРЕЖДЕНИЕ
            control.MouseMove += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.KeyPress += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.Click += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.MouseClick += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.MouseDown += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.MouseUp += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.KeyDown += (s, e) => { if (!_isWarningShowing) ResetTimer(); };
            control.KeyUp += (s, e) => { if (!_isWarningShowing) ResetTimer(); };

            foreach (Control child in control.Controls)
            {
                SubscribeToActivityEvents(child);
            }
        }

        public static void ResetTimer()
        {
            if (_timer != null && !_isWarningShowing)
            {
                _timer.Stop();
                _timer.Start();
            }

            // НЕ ЗАКРЫВАЕМ ФОРМУ ПРЕДУПРЕЖДЕНИЯ АВТОМАТИЧЕСКИ
        }

        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            CloseWarningForm();
        }

        private static void CloseWarningForm()
        {
            if (_warningForm != null && !_warningForm.IsDisposed)
            {
                _isWarningShowing = false;
                _warningForm.Close();
                _warningForm.Dispose();
                _warningForm = null;
            }

            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Dispose();
                _countdownTimer = null;
            }
        }

        public static void UpdateTimeout(int newTimeoutSeconds)
        {
            _timeoutSeconds = newTimeoutSeconds;
            if (_timer != null)
            {
                _timer.Interval = _timeoutSeconds * 1000;
                ResetTimer();
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            // Показываем кастомное окно предупреждения
            ShowWarningForm();
        }

        private static void ShowWarningForm()
        {
            // Если уже открыто окно, не открываем новое
            if (_warningForm != null && !_warningForm.IsDisposed)
            {
                return;
            }

            _isWarningShowing = true; // Устанавливаем флаг, что показывается предупреждение

            _warningForm = new Form();
            _warningForm.Text = "Предупреждение о бездействии";
            _warningForm.Size = new Size(450, 220);
            _warningForm.StartPosition = FormStartPosition.CenterParent;
            _warningForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            _warningForm.MaximizeBox = false;
            _warningForm.MinimizeBox = false;
            _warningForm.TopMost = true;
            _warningForm.ControlBox = true;
            _warningForm.ShowInTaskbar = false;

            Label lblMessage = new Label()
            {
                Text = $"Вы неактивны более {_timeoutSeconds} секунд.\n\n" +
                       $"Система будет заблокирована через {_countdownSeconds} секунд.\n\n" +
                       $"Нажмите 'Продолжить' чтобы остаться в системе.",
                Location = new Point(20, 30),
                Size = new Size(400, 90),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 10)
            };

            Button btnContinue = new Button()
            {
                Text = "Продолжить",
                Location = new Point(175, 140),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            btnContinue.Click += (s, e) =>
            {
                CloseWarningForm();
                _isWarningShowing = false;
                ResetTimer();
            };

            _warningForm.Controls.Add(lblMessage);
            _warningForm.Controls.Add(btnContinue);

            // Обработчик закрытия формы
            _warningForm.FormClosed += (s, e) =>
            {
                _isWarningShowing = false;
            };

            // Показываем форму с центрированием относительно родителя
            _warningForm.Show(_currentForm);

            // Запускаем обратный отсчет
            StartCountdown(lblMessage);
        }

        private static void StartCountdown(Label lblMessage)
        {
            int remainingSeconds = _countdownSeconds;

            _countdownTimer = new Timer();
            _countdownTimer.Interval = 1000;
            _countdownTimer.Tick += (s, e) =>
            {
                remainingSeconds--;

                if (_warningForm != null && !_warningForm.IsDisposed)
                {
                    // Обновляем сообщение
                    lblMessage.Text = $"Вы неактивны более {_timeoutSeconds} секунд.\n\n" +
                                     $"Система будет заблокирована через {remainingSeconds} секунд.\n\n" +
                                     $"Нажмите 'Продолжить' чтобы остаться в системе.";

                    if (remainingSeconds <= 0)
                    {
                        _countdownTimer.Stop();
                        CloseWarningForm();

                        // Блокируем систему
                        _onInactivity?.Invoke();
                    }
                }
                else
                {
                    _countdownTimer.Stop();
                }
            };

            _countdownTimer.Start();
        }
    }
}
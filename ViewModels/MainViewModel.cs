using System.Collections.ObjectModel;
using System.Windows.Input;
using ChessApp.Models;

namespace ChessApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // ---------- Коллекция фигур ----------
        public ObservableCollection<ChessPiece> Pieces { get; } = new();

        // ---------- Выбранная фигура ----------
        private ChessPiece? _selectedPiece;
        public ChessPiece? SelectedPiece
        {
            get => _selectedPiece;
            set
            {
                if (SetProperty(ref _selectedPiece, value))
                    OnPropertyChanged(nameof(SelectedPieceInfo));
            }
        }

        public string SelectedPieceInfo =>
            _selectedPiece?.ToString() ?? "Фигура не выбрана";

        // ---------- Поле ввода хода ----------
        private string _targetInput = string.Empty;
        public string TargetInput
        {
            get => _targetInput;
            set => SetProperty(ref _targetInput, value);
        }

        // ---------- Лог ----------
        private string _log = string.Empty;
        public string Log
        {
            get => _log;
            private set => SetProperty(ref _log, value);
        }

        // ---------- Команды ----------
        public ICommand MoveCommand { get; }
        public ICommand ResetCommand { get; }

        // ---------- Конструктор ----------
        public MainViewModel()
        {
            MoveCommand = new RelayCommand(ExecuteMove);
            ResetCommand = new RelayCommand(ExecuteReset);

            // Инициализация значений — вне самих классов фигур
            InitializePieces();
        }

        private void InitializePieces()
        {
            Pieces.Clear();

            // Создание фигур с начальными позициями
            Pieces.Add(new Queen(PieceColor.White, new BoardPosition(4, 1)));
            Pieces.Add(new Rook(PieceColor.White, new BoardPosition(1, 1)));
            Pieces.Add(new Rook(PieceColor.White, new BoardPosition(8, 1)));
            Pieces.Add(new Bishop(PieceColor.White, new BoardPosition(3, 1)));
            Pieces.Add(new Bishop(PieceColor.White, new BoardPosition(6, 1)));

            Pieces.Add(new Queen(PieceColor.Black, new BoardPosition(4, 8)));
            Pieces.Add(new Rook(PieceColor.Black, new BoardPosition(1, 8)));
            Pieces.Add(new Rook(PieceColor.Black, new BoardPosition(8, 8)));
            Pieces.Add(new Bishop(PieceColor.Black, new BoardPosition(3, 8)));
            Pieces.Add(new Bishop(PieceColor.Black, new BoardPosition(6, 8)));

            SelectedPiece = null;
            AppendLog("Фигуры расставлены на начальные позиции.");
        }

        private void ExecuteMove()
        {
            if (SelectedPiece is null)
            {
                AppendLog("Ошибка: выберите фигуру.");
                return;
            }

            if (!BoardPosition.TryParse(TargetInput, out BoardPosition target))
            {
                AppendLog($"Ошибка: «{TargetInput}» — некорректная позиция. Формат: A1–H8.");
                return;
            }

            BoardPosition from = SelectedPiece.Position;
            bool success = SelectedPiece.TryMove(target);

            if (success)
            {
                AppendLog($"{SelectedPiece.Name} ({SelectedPiece.Color}): {from} → {target} ✓");
                OnPropertyChanged(nameof(SelectedPieceInfo));

                // Обновим список, чтобы отображение позиции обновилось
                int index = Pieces.IndexOf(SelectedPiece);
                if (index >= 0)
                {
                    ChessPiece piece = Pieces[index];
                    Pieces.RemoveAt(index);
                    Pieces.Insert(index, piece);
                    SelectedPiece = piece;
                }
            }
            else
            {
                AppendLog($"{SelectedPiece.Name} ({SelectedPiece.Color}): {from} → {target} ✗ (недопустимый ход)");
            }
        }

        private void ExecuteReset()
        {
            InitializePieces();
        }

        private void AppendLog(string message)
        {
            Log = string.IsNullOrEmpty(Log)
                ? message
                : message + "\n" + Log;
        }
    }
}

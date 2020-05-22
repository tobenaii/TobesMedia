using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TobesMediaCore.Data.Media;
using TobesMediaCore.Network;

namespace TobesMediaWindowsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Movie> movies = new List<Movie>();
        private List<Task<Movie>> moviesLoading = new List<Task<Movie>>();
        private string m_currentSearch;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        public async void LoadMovie(string imdbID)
        {
            Task<Movie> movieTask = new MediaBaseRequest().GetMovie(imdbID);
            moviesLoading.Add(movieTask);
            Movie movie = await movieTask;
            movies.Add(movie);
            MediaGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_currentSearch = (sender as TextBox).Text;
            MediaGrid.ColumnDefinitions.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace Adventure
{
    public partial class Adventure : Form
    {
        private Player _player;

        public Adventure()
        {
            InitializeComponent();

            _player = new Player();

            _player.CurrentHitpoints = 10;
            _player.MaximumHitpoints = 10;
            _player.Gold = 20;
            _player.ExperiencePoints = 0;
            _player.Level = 1;

            lblHitpoints.Text = _player.CurrentHitpoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
        }
    }
}

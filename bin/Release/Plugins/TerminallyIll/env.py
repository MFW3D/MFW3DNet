import clr
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("WorldWind")
from WorldWind import MainApplication
from System.Windows.Forms import Control
app = Control.FromHandle( MainApplication.GetWWHandle() )
world = app.WorldWindow.CurrentWorld


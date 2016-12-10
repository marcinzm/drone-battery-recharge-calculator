##############################################
# Drone Battery Recharge Calculator          #
# The author is Marcin Zmyslowski.           #
# Contact: marcinzmyslowski@poczta.onet.pl   #
##############################################

1) Copyright:
You can use for commercial and uncommercial use.
If you like the software and makes it helpful for you, please donate via PayPal (email: marcinzmyslowski@poczta.onet.pl) or 
visit my web page: http://wynajmijdronaod.zmyslowski.pl/programy - there is information how to donate..

2) Goal:
Drone Battery Recharge Calculator is the software which would be helpful
for DJI drone operators who don`t fly everyday and care for their DJI batteries
and store them in 40% charge capacity. This is right approach for storage of drone batteries.

3) Installation:

a) download the DroneBatteryLoadCalculator folder with all files and subfolders from github

b) Please create a folder DroneBatteryLoadCalculator in i.e. c:\ or d:\ drive. Cope the installation files into that folder

c) add the this c:\DroneBatteryLoadCalculator\bin\Release\ folder to Windows environment to make the application run globally on every place of your drive

d) If you use Total Commander please type gl.exe on any drive and any folder you currently are,
the software should run, if doesn`t run please install the latest Windows .NET framework package from microsoft.com web page.

e) You can change the language settings from Polski (Polish) to English.

2) Bugs
If you find that the recharging time is not accurate please email it, and I will add some changes.

I have tested out the recharing time on DJI Phantom 3 Pro and 2 drone batteries and the calculating recharing time
for this drone is the most accurate. The rest recharging times are my calculation



IF THIS SOFTWARE MAKES YOUR WORK AND ENJOY HELPFUL PLEASE DONATE via PayPal to my mail: marcinzmyslowski@poczta.onet.pl or visit my web page: http://wynajmijdronaod.zmyslowski.pl/programy - there is information how to donate.

if you want to contact with me, email me, I will reply for sure.

Marcin Zmyslowski from Poland


<form action="https://www.paypal.com/cgi-bin/webscr" method="post" target="_top">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----
">
<input type="image" src="https://www.paypalobjects.com/pl_PL/PL/i/btn/btn_donateCC_LG.gif" border="0" name="submit" alt="PayPal – Płać wygodnie i bezpiecznie">
<img alt="" border="0" src="https://www.paypalobjects.com/pl_PL/i/scr/pixel.gif" width="1" height="1">
</form>

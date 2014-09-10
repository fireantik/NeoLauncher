NeoLauncher
===========

Launcher/patcher for Tribes Vengeance

All of the urls are configurable in settings file without need for recompilation

Pulls news from http://tribesrevengeance.com/patcher/news.html

Pulls info from http://tribesrevengeance.com/patcher/info.xml

Info is xml document with 
```
<?xml version="1.0" encoding="UTF-8"?>
<Info>
	<Version>1</Version>
	<DownloadUrl>http://tribesrevengeance.com/patcher/1.zip</DownloadUrl>
</Info>
```

If version > version in settings => download file from DownloadUrl and extract it into root tribes directory.

Wont execute if not in the same directory as tv_cd_dvd.exe

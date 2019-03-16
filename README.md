# UnB WebCrawler
 It is intended to be a helpfull tool towards you getting your UnB¹ grade fullfiled with the disciplines you would like to study but you wouldn't even see it is available by yourself.

 The crawler collects all the disciplines available for enrollment and displays all key data to an excel, where you can organize yourself to either set all your grade, or fulfill the the schedule gaps.

## How it works
 It is a console application in which at its start, two parallel tasks are created. 
   * The first one does the dirty work of naviganting through the [UnB website](https://matriculaweb.unb.br/graduacao/oferta_dep.aspx?cod=1) and its departments, offers, disciplines and classes. To gather all key data required for the studant to make a good and mindful choice on his/her available options for enrollment.
   * The second keeps waiting for each department the other thread finishes and output the all collected data to an excel sheet. When it is done, it waits for the next department to finish until it is all set.

## Examples
 

## Requirements
  * Microsoft Office (used to output excel)
  * Visual Studio

## Built with
  * C#
  * ASP.NET Core


¹ Universidade de Brasília, Brasil

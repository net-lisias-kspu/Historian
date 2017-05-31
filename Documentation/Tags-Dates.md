# Date and Time Related Tags

* `<Date(format)>` Formatted date string. Standard Earth calendar dates if game settings is Earth time (24 hour days). "Fake" Kerbin dates based on 12 alternating 35 and 36 day months and six day week if game settings is Kerbin time (6 hour days). e.g. Wednesday 13 January 2016 or Esant 06 Trenam 001. __NOTE__: Kerbin day and month names can be customised from the settings window - see below for default values.
* `<DateKAC(format)>` Formatted date string using variable length year †
* `<RealDate(format)>` the real world date and time.

* `<UT>` KSP Universal Time. Example: _Y12, D29, 2:02:12_
* `<Year>` Current year in chosen `CalendarMode`
* `<YearKAC>` Current year using variable length year †
* `<Day>` Current day in chosen `CalendarMode`
* `<DayKAC>` Current day using variable length year †
* `<Hour>` Current hour in chosen `CalendarMode`
* `<Minute>` Current minute in chosen `CalendarMode`
* `<Second>` Current second in chosen `CalendarMode`
* `<T+>` Current mission time for the active vessel, in chosen `CalendarMode` (Only available in Flight Mode). Example: _T+ 2y, 23d, 02:04:12_
* `<MET>` Alias for `<T+>`

† Note for Earth calendar dates the game calculates the clock date using fixed 365 day years taking no account of leap years. [Kerbal Alarm Clock](http://forum.kerbalspaceprogram.com/index.php?/topic/22809-11x-kerbal-alarm-clock-v3610-april-25/) and [RSS Date Time Formatter](http://forum.kerbalspaceprogram.com/index.php?/topic/139335-ksp-112-rss-datetime-formatter-v10/) calculates based on seconds since start date and takes account of leap years. [RSS](http://forum.kerbalspaceprogram.com/index.php?/topic/50471-112-real-solar-system-v1110-may-1/) only shows the planets in the correct relative locations for an historical date if leap years are accounted for. The different `<Date>` and `<DateKAC>` tags allow you to choose which of these calendar schemes you wish to use.


--
### [Documentation Index](../README.md)
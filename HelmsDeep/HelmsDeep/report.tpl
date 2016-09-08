
<body>

<h1>Температура</h1>
<table border=5 bordercolor="ffffff">
<tr bgcolor="aaaaaa">
	<th>Минимальная</th>
	<th>Средняя</th>
	<th>Максимальная</th>
</tr>
<tr>
	<td>{Min(ModuleTempSensor,0)}</td>
	<td>{Avg(ModuleTempSensor,0)}</td>
	<td>{Max(ModuleTempSensor,0)}</td>
</tr>
</table>


<h2>График температуры:</h2>
<p align="left"><img src='{Diagram(ModuleTempSensor,0)}'/></p>

<h2>Диагностическая информация</h2>
<p>Количество включений: {Count(ServiceOn,0)}</p>
<p>Количество выключений: {Count(ServiceOff,0)}</p>
<p>Количество измерений температуры: {Count(ModuleTempSensor,0)}</p>
  
</body>
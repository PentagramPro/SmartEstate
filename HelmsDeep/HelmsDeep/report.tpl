
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
<p align="left">{Diagram(ModuleTempSensor,0)}</p>
  
</body>
#  車両管理DBアクセスアプリ

import pandas as pdcsv  #CSVモジュールを利用
import sqlite3  #sqlite3モジュールを利用

data=pdcsv.read_csv('101.csv',encoding='utf-8')
data.info()

for index, row in data.iterrows():
    print(index)
    print(row)
    


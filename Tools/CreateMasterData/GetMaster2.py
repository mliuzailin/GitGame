# -*- coding: utf-8 -*-
# pip install requests
# pip install requests_toolbelt

import os
import sys
import json
import time
import types

from datetime import datetime

import requests
from requests_toolbelt.utils import dump
import urllib.parse

from lib import ApiAuthUser
from lib import ApiGetMasterData2
from lib import SecureUtil
from lib import QueryUtile
from lib import ServerSeting

debug_log = False

argvs = sys.argv  # コマンドライン引数を格納したリストの取得
argc = len(argvs)  # 引数の個数

# global変数
uuid = ""
api_url = ""
sqlite3_filepath = ""
createtable_filepath = ""
createindex_filepath = ""
selectserver = ""
server_version = ""

pqdmsessid = ""
ott = ""
servername = ""


# パラメータチェック
if (argc != 2 and argc != 3):   # 引数が足りない場合は、その旨を表示
    print('Usage: # python %s servertype ' % argvs[0])
    print('Usage: # python %s servertype(culom) servertype(data) ' % argvs[0])
    sys.exit('')


# global変数初期化
def initalizeSetting(servertype):
    global uuid
    global api_url
    global sqlite3_filepath
    global sqlite3_zero_filepath
    global createtable_filepath
    global createindex_filepath
    global selectserver
    global server_version

    global pqdmsessid
    global ott
    global servername

    uuid = ServerSeting.UUID(servertype)
    api_url = ServerSeting.ApiUrl(servertype)
    sqlite3_filepath = ServerSeting.Sqlite3Path(servertype)
    sqlite3_zero_filepath = ServerSeting.Sqlite3ZeroPath(servertype)
    createtable_filepath = ServerSeting.CreateTable(servertype)
    createindex_filepath = ServerSeting.CreateIndex(servertype)
    selectserver = ServerSeting.SelectServerType(servertype)
    server_version = ServerSeting.ApiVersion(servertype)

    pqdmsessid = ""
    ott = ""
    servername = servertype


# login処理
def login():
    loginUrl = api_url + ApiAuthUser.api()
    print("===============================")
    print("login: " + servername)
    print(loginUrl)
    print("===============================")

    respons = requests.post(
        loginUrl,
        headers=ApiAuthUser.headers(),
        data=ApiAuthUser.data(uuid, server_version)
    )

    if debug_log == True:
        print("-------------")
        print("loginUrl: " + loginUrl)
        data = dump.dump_all(respons)
        print(data.decode('utf-8'))
        print("-------------")

    print(loginUrl + " status: " + str(respons.status_code))

    if respons.status_code != 200:
        sys.exit("respons.status_code: " + str(respons.status_code))

    SecureUtil.updateCookie(respons.headers)

    return SecureUtil.updateSession(respons.text)


def createsqlite3(path, zero_mode):
    masternamelist = ApiGetMasterData2.masterNameList()
    QueryUtile.removeTable(path, masternamelist)
    QueryUtile.createTable(path, masternamelist)
    QueryUtile.createTableSetting(path, createtable_filepath)
    QueryUtile.createIndexSetting(path, createindex_filepath)
    if zero_mode == True:
        QueryUtile.insertTable(
            path, ApiGetMasterData2.masterZeroNameList())
    else:
        QueryUtile.insertTable(path, masternamelist)

    nowTime = datetime.now().strftime('%s')

    if zero_mode == True:
        nowTime = str(0)

    sql = 'insert into client_cleate_type_master(create_type, create_time) values(' + str(
        selectserver) + ',' + nowTime + ')'
    QueryUtile.runSql(path, sql)

    if zero_mode == True:
        sql = 'update text_definition_master set tag_id=0'
        QueryUtile.runSql(path, sql)
        sql = 'update global_params_master set tag_id=0'
        QueryUtile.runSql(path, sql)


initalizeSetting(argvs[1])

#print(ApiGetMasterData2.data(uuid,str("0"), server_version))
"""
masternamelist = ApiGetMasterData2.masterNameList();
QueryUtile.removeTable(sqlite3_filepath, masternamelist)
QueryUtile.createTable(sqlite3_filepath, masternamelist)
QueryUtile.createTableSetting(sqlite3_filepath, createtable_filepath)
QueryUtile.createIndexSetting(sqlite3_filepath, createindex_filepath)
QueryUtile.insertTable(sqlite3_filepath, masternamelist)
sys.exit()
"""

print("===============================")
print("start")
print("===============================")

# login
pqdmsessid, ott = login()

# getmaster2
getMaster2Url = api_url + ApiGetMasterData2.api()
debugGetMaster2URl = api_url + ApiGetMasterData2.debugApi()
mastertypes = ApiGetMasterData2.masterTypes()
print("===============================")
print("getmaster2 column")
print(getMaster2Url)
print("===============================")

for mtype in mastertypes:

    # スキーマの取得
    respons = requests.post(
        debugGetMaster2URl,
        headers=ApiGetMasterData2.headers(pqdmsessid),
        data=ApiGetMasterData2.data(uuid, str(ott), mtype, -1, server_version)
    )

    if debug_log == True:
        print("-------------")
        data = dump.dump_all(respons)
        print(data.decode('utf-8'))
        print("-------------")

    print("table: " + debugGetMaster2URl +
          " status: " + str(respons.status_code))

    if respons.status_code != 200:
        sys.exit("respons.status_code: " + str(respons.status_code))

    get_dict = json.loads(respons.text)
    jsonkey = ApiGetMasterData2.masterArray(mtype)
    master = ApiGetMasterData2.masterName(mtype)

    #QueryUtile.tempsave(get_dict, master)
    QueryUtile.saveSchema(get_dict, jsonkey, master)
    pqdmsessid, ott = SecureUtil.updateSession(respons.text)


# データ取得用のサーバ切り替え
if (argc == 3):
    initalizeSetting(argvs[2])
    pqdmsessid, ott = login()
    getMaster2Url = api_url + ApiGetMasterData2.api()

print("===============================")
print("getmaster2 data")
print(getMaster2Url)
print("===============================")

for mtype in mastertypes:
    # データの取得
    respons = requests.post(
        getMaster2Url,
        headers=ApiGetMasterData2.headers(pqdmsessid),
        data=ApiGetMasterData2.data(uuid, str(ott), mtype, 0, server_version)
    )

    if debug_log == True:
        print("-------------")
        data = dump.dump_all(respons)
        print(data.decode('utf-8'))
        print("-------------")

    print("data: " + getMaster2Url + " status: " + str(respons.status_code))

    if respons.status_code != 200:
        sys.exit("respons.status_code: " + str(respons.status_code))

    get_dict = json.loads(respons.text)
    jsonkey = ApiGetMasterData2.masterArray(mtype)
    master = ApiGetMasterData2.masterName(mtype)

    #QueryUtile.tempsave(get_dict, master)
    QueryUtile.save(get_dict, jsonkey, master)
    pqdmsessid, ott = SecureUtil.updateSession(respons.text)


# create sqlite3
print("create sqlite3")
createsqlite3(sqlite3_filepath, False)

if os.path.isfile(sqlite3_zero_filepath):
    print("create sqlite3 zero")
    createsqlite3(sqlite3_zero_filepath, True)

print("finish")

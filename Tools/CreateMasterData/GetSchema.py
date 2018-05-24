# -*- coding: utf-8 -*-
# pip install requests
# pip install requests_toolbelt

import sys
import json

import requests
from requests_toolbelt.utils import dump

from lib import ApiAuthUser
from lib import ApiGetSchema
from lib import SecureUtil
from lib import ServerSeting

debug_log = False

argvs = sys.argv  # コマンドライン引数を格納したリストの取得
argc = len(argvs)  # 引数の個数

if (argc != 3):   # 引数が足りない場合は、その旨を表示
    sys.exit('Usage: # python %s servertype' % argvs[0])

uuid = ServerSeting.UUID(argvs[1])
api_url = ServerSeting.ApiUrl(argvs[1])
server_version = ServerSeting.ApiVersion(argvs[1])
path = argvs[2]

pqdmsessid = ""
ott = ""

print("start")

# login
print("login")
loginUrl = api_url + ApiAuthUser.api()

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

pqdmsessid, ott = SecureUtil.updateSession(respons.text)

# getmaster2
debugGetMasterSchemaURl = api_url + ApiGetSchema.api()
print("GetSchema: " + debugGetMasterSchemaURl)

# スキーマの取得
respons = requests.post(
    debugGetMasterSchemaURl,
    headers=ApiGetSchema.headers(pqdmsessid),
    data=ApiGetSchema.data(uuid, str(ott), server_version)
)

if debug_log == True:
    print("-------------")
    data = dump.dump_all(respons)
    print(data.decode('utf-8'))
    print("-------------")

get_dict = json.loads(respons.text)

f = open(path, 'w')  # 書き込みモードで開く
f.write(get_dict["result"])  # 引数の文字列をファイルに書き込む
f.close()  # ファイルを閉じる

print("finish")

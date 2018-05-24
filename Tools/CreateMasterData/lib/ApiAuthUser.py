# -*- coding: utf-8 -*-
import json
import pickle

from lib import SecureUtil

user_api = 'auth_user.php'

user_json = '''
{
    "header" : {
        "api_version" : "REPLACE_APIVERSION",
        "packet_unique_id" : 53,
        "terminal"         : {
            "platform" : 0,
            "name"     : "iPhone6",
            "os"       : "iPhone OS 8.0"
        },
        "rank"             : 0,
        "ad_id"            : "",
        "ad_flag"          : 0,
        "ott"              : 0,
        "local_time"       : 1509014872
    },
    "terminal" : {
        "platform" : 0,
        "name"     : "iPhone6",
        "os"       : "iPhone OS 8.0"
    },
    "boot"     : null,
    "uuid"     : ""
}
'''


def api():
    return user_api


def headers():
    user_headers = SecureUtil.basehaders()
    if isinstance(user_headers.get('Cookie'), type(None)) == False:
        del user_headers['Cookie']

    return user_headers


def data(uuid, version):
    dst_json = user_json.replace('REPLACE_APIVERSION', version)
    auth_user_dict = json.loads(dst_json)
    auth_user_text = SecureUtil.createhaders(auth_user_dict, uuid)
    csum_ans = SecureUtil.csum(auth_user_text + uuid)

    auth_user_data = 'request={0}&csum={1}&ott=0'.format(
        auth_user_text, csum_ans)

    return auth_user_data

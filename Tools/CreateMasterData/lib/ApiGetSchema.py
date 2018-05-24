# -*- coding: utf-8 -*-
import json
import pickle
from enum import IntEnum

from lib import SecureUtil

user_api = 'debug_get_master_schema.php'

user_json = '''
{
  "header": {
    "api_version": "REPLACE_APIVERSION",
    "packet_unique_id": 54,
    "terminal"         : {
      "platform" : 0,
      "name"     : "iPhone6",
      "os"       : "iPhone OS 8.0"
    },
    "rank": 1,
    "ad_id": "",
    "ad_flag": 0,
    "ott": 0,
    "local_time": 1509014872
  },
  "achievement_viewed": null
}
'''


def api():
    return user_api


def headers(session):
    user_headers = SecureUtil.basehaders()
    user_headers['Cookie'] = "; PQDMSESSID=" + session
    return user_headers


def data(uuid, ott, version):
    dst_json = user_json.replace('REPLACE_APIVERSION', version)
    auth_user_dict = json.loads(dst_json)

    hash = []
    item = {"hash": "", "timing": 0}
    hash.append(item)
    auth_user_dict["hash"] = hash

    """
  for emasterdata in emasterdata_server:
    item = {"type": emasterdata.value , "hash": "", "timing": 0, "tag_id": 0}
    hash.append(item)

  auth_user_dict["hash"] = hash
  """

    auth_user_text = SecureUtil.createhaders(auth_user_dict, uuid)
    csum_ans = SecureUtil.csum(auth_user_text + uuid)

    auth_user_data = 'request={0}&csum={1}&ott={2}'.format(
        auth_user_text, csum_ans, ott)

    return auth_user_data

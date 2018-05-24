# -*- coding: utf-8 -*-
import random
import datetime
import time
import json
import hashlib
import re

user_headers = {
    'Accept': '*/*',
    'Accept-Encoding': 'gzip, '
    'deflate',
    'Accept-Language': 'ja-jp',
    'Connection': 'keep-alive',
    'Content-Type': 'application/x-www-form-urlencoded',
    'Proxy-Connection': 'keep-alive',
    'User-Agent': 'PQDM-Client-App',
    'X-Unity-Version': '5.5.0f3'
}

packet_unique_id = 60
cookie_awselb_key = "AWSELB"
cookie_awselb_value = ""


def basehaders():
    return user_headers


def createhaders(user_dict, uuid=""):
    if(len(uuid) > 0):
        user_dict["uuid"] = uuid

    now = datetime.datetime.now()
    user_dict["header"]["local_time"] = int(time.mktime(now.timetuple()))

    global packet_unique_id
    user_dict["header"]["packet_unique_id"] = int(packet_unique_id)
    packet_unique_id += 1

    user_text = str(user_dict).replace(" ", "")
    user_text = user_text.replace("'", "\"")
    user_text = user_text.replace("None", "null")

    return user_text


def csum(text):
    return "00000000"


def updateCookie(dict):
    plane_cookie = dict.get('set-cookie')

    if plane_cookie is None:
        print("none set-cookie")
        return

    dict_cookie = {}
    cookies = re.split('[,;]', plane_cookie)
    for c in cookies:
        cookie = c.split("=")
        key = cookie[0].strip()
        value = cookie[1].strip()
        dict_cookie[key] = value

    if isinstance(dict_cookie.get(cookie_awselb_key), type(None)):
        print("none use key: " + cookie_awselb_key)
        return

    global cookie_awselb_value
    cookie_awselb_value = dict_cookie[cookie_awselb_key]
    print("use key:" + cookie_awselb_key + " value: " + cookie_awselb_value)


def getAwselb():
    return (cookie_awselb_value)


def updateSession(text):
    # アクセス用の情報取得
    get_dict = json.loads(text)
    pqdmsessid = get_dict["header"]["session_id"]
    ott = get_dict["header"]["ott"]

    # print("session_id: " + pqdmsessid)
    # print("ott: " + str(ott))

    return (pqdmsessid, ott)


def log(text):
    return
    print("csum-> " + text)

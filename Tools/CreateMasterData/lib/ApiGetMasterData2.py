# -*- coding: utf-8 -*-
import json
import pickle
from enum import IntEnum

from lib import SecureUtil

user_api = 'get_master2.php'

debug_user_api = 'debug_get_master2.php'

user_json = '''
{
  "header": {
    "api_version": "REPLACE_APIVERSION",
    "packet_unique_id": 57,
    "terminal"         : {
        "platform" : 0,
        "name"     : "iPhone6",
        "os"       : "iPhone OS 8.0"
    },
    "rank": 3,
    "ad_id": "",
    "ad_flag": 0,
    "ott": 0,
    "local_time": 1509014872
  },
  "achievement_viewed": null
}
'''

# マスターデータタイプ（サーバー側ID）


class emasterdata_server(IntEnum):
    RANK = 10,                  # ユーザーランク
    DEFAULT_PARTY = 11,         # デフォルトパーティー
    # NONE 12-19
    CHARA = 20,                 # キャラパラメータ
    CHARA_EVOL = 21,            # キャラクター進化情報
    SKILL_LEADER = 22,          # リーダースキル
    SKILL_ACTIVE = 23,          # アクティブスキル
    SKILL_PASSIVE = 24,         # パッシブスキル
    SKILL_LIMITBREAK = 25,      # リミットブレイクスキル
    SKILL_BOOST = 26,           # ブーストスキル / 26:スキルブースト★
    # NONE 27-29
    # ENEMY = 30,                 # エネミーパラメータ 開始時に取得？
    ENEMY_GROUP = 31,           # エネミーグループ
    GUERRILLA_BOSS = 32,        # ゲリラボス
    ENEMY_ACTION_TABLE = 33,    # 敵行動テーブル / 33:敵行動パターン★
    ENEMY_ACTION_PARAM = 34,    # 敵行動定義
    STATUS_AILMENT = 35,        # 状態異常整理用定義 / 35:状態変化定義
    # NONE 36-39
    AREA = 40,                  # エリア情報
    # QUEST = 41,                 # クエスト情報
    # QUEST_FLOOR = 42,           # クエスト内フロア情報
    # CATEGORY_PATTERN = 43,     # 階層内ランダムカテゴリ分布パターン
    # EXPECT_PATTERN = 44,       # 階層内期待値分布パターン
    # PANEL = 45,                 # パネル効果パラメータ
    # PANEL_GROUP = 46,          # パネル分岐グループ
    QUEST_REQUIREMENT = 47,     # クエスト入場条件
    # 48-59
    GACHA = 60,                 # ガチャ定義 / 60:ガチャ定義//※差分取得ではない GetMasterApiと同じロジック
    # GACHA_ASSIGN = 61,         # ガチャ詳細アサイン
    GACHA_GROUP = 62,           # ガチャグループ定義 / 62:ガチャ定義
    # NONE 63-69
    # LOGIN_EVENT = 70,          # 通算ログイン
    # LOGIN_CHAIN = 71,          # 連続ログイン
    # LOGIN_EVENT = 72,          # 期間限定ログイン
    EVENT = 73,                 # 期間限定イベント / 73:期間限定イベント//※差分取得ではない GetMasterApiと同じロジック
    AREA_AMEND = 74,            # 期間限定エリア補正
    PRESENT = 75,               # プレゼント定義 / 75:プレゼント
    # ACHIEVEMENT_LIST = 76,      # アチーブメント APIがありそちらで取得
    PRESENT_GROUP = 77,        # プレゼント定義 / 77:プレゼントグループ
    # NONE 78 - 79
    STORE_PRODUCT = 80,         # ストア商品一覧
    ASSET_PATH = 81,            # AssetBundleパス / 81:AssetBundleパスデータ
    INFORMATION = 82,           # 運営通知 / 82:運営のお知らせ//※差分取得ではない
    # STORE_PRODUCT_EVENT = 83,   # ストアイベント情報 / ストアイベント情報
    # LOGIN_MONTHLY = 84,        # 月間ログイン / カレンダー
    NOTIFICATION = 85,          # ローカル通知 / 85:プッシュ通知//※差分取得ではない GetMasterApiと同じロジック
    BEGGINER_BOOST = 86,        # 初心者ブースト
    AREA_CATEGORY = 87,         # エリアカテゴリ情報
    # INVITATION = 88,           # 招待イベント定義
    ENEMY_ABILITY = 89,         # エネミー特性
    TEXT_DEFINITION = 90,       # テキスト定義
    LINK_SYSTEM = 91,           # リンクシステム
    TOPPAGE = 92,               # トップページ
    AUDIO_DATA = 93,            # オーディオ再生情報 / サウンド再生情報
    # POINT_SHOP_PRODUCT = 94,    # ポイントショップ商品定義
    GACHA_TICKET = 95,          # ガチャチケット
    LIMIT_OVER = 96,            # 限界突破
    ITEM = 97,                  # 消費アイテム
    GLOBAL_PARAMS = 98,         # マスターデータタイプ：共通定義 / グローバルパラメーター
    # NONE 99
    QUEST_KEY = 100,            # クエストキー
    #ACHIEVEMENT_GROUP =101,    #
    WEB_VIEW = 102,               # WebView表示
    HERO = 103,                 # 主人公
    HERO_LEVEL = 104,           # 主人公レベル
    #SKILL_ACTIVE_HERO = 105,   #
    HERO_ADD_EFFECT_RATE = 106,
    # TOPIC_INFORMATION=107,     # トピック:専用APIあり
    STORY = 108,                # ストーリー
    # EVENT_POINT=109,           # イベントポイント定義 ACQv400//※170403現在モック
    # GROSSING_PRESENT=110,      # 総付けガチャ ACQv400//※170403現在モック
    ENEMY_HATE = 111,           # 敵ヘイト
    RENEW_QUEST = 112,          # 新クエスト
    NPC = 113,                  # NPC
    STORY_CHARA = 114,          # ストーリー用キャラ設定
    ILLUSTRATOR = 115,          # イラストレータ名
    REGION = 118,               # リージョン※
    RENEW_QUEST_SCORE = 119,    # クエストスコア
    PLAY_SCORE = 120,           # プレイスコア
    SCORE_EVENT = 121,          # イベントスコア
    SCORE_REWARD = 122,         # スコア報酬
    QUEST_APPEARANCE = 123,     # クエスト演出差替え情報※
    STEP_UP_GACHA = 125,        # ステップアップガチャ情報
    STEP_UP_GACHA_MANAGE = 126,  # ステップアップガチャ管理情報
    CHALLENGE_QUEST = 127,      # 成長ボスクエスト
    CHALLENGE_EVENT = 128,      # 成長ボスイベント
    CHALLENGE_REWARD = 129,     # 成長ボス報酬
    GACHA_TEXT = 131,           # ガチャテキスト管理
    GACHA_TEXT_REF = 132,       # ガチャテキスト参照管理
    GENERAL_WINDOW = 133,       # 汎用ウィンドウ管理


def reservedMaster():
    return ["sqlite_sequence", ]


def masterTypes():
    return list(emasterdata_server)


def masterArray(mtype):
    name = mtype.name.lower()

    # if mtype.value == emasterdata_server.ACHIEVEMENT_LIST.value:
    #  name = "achievement"
    if mtype.value == emasterdata_server.AUDIO_DATA.value:
        name = "audiodata"
    elif mtype.value == emasterdata_server.GACHA_TICKET.value:
        name = "gachaticket"
    elif mtype.value == emasterdata_server.WEB_VIEW.value:
        name = "webview"

    return "master_array_" + name


def masterName(mtype):
    name = mtype.name.lower()

    if mtype.value == emasterdata_server.RANK.value:
        name = "user_" + name + "_master"
    elif mtype.value == emasterdata_server.BEGGINER_BOOST.value:
        name = "beginner_boost" + "_master"
    elif mtype.value == emasterdata_server.ASSET_PATH.value:
        name = "asset_bundle_path" + "_master"
    elif mtype.value == emasterdata_server.QUEST_REQUIREMENT.value:
        name = name
    elif mtype.value == emasterdata_server.TOPPAGE.value:
        name = "top_page" + "_master"
    else:
        name = name + "_master"
    # elif mtype.value == emasterdata_server.ACHIEVEMENT_LIST.value:
    #  name = "achievement" + "_master"

    return name


def masterNameList():
    list = []
    for mtype in masterTypes():
        list.append(masterName(mtype))

    for name in reservedMaster():
        list.append(name)

    return list


def masterZeroNameList():
    list = []
    list.append(masterName(emasterdata_server.TEXT_DEFINITION))
    list.append(masterName(emasterdata_server.GLOBAL_PARAMS))
    return list


def api():
    return user_api


def debugApi():
    return debug_user_api


def headers(session):
    user_headers = SecureUtil.basehaders()
    user_headers['Cookie'] = "; PQDMSESSID=" + session

    if len(SecureUtil.cookie_awselb_value):
        user_headers['Cookie'] += "; " + SecureUtil.cookie_awselb_key + "=" + \
            SecureUtil.cookie_awselb_value

    return user_headers


def data(uuid, ott, mtype, tab_id, version):
    dst_json = user_json.replace('REPLACE_APIVERSION', version)
    auth_user_dict = json.loads(dst_json)

    hash = []
    item = {"type": mtype.value, "hash": "", "timing": 0, "tag_id": tab_id}
    hash.append(item)
    auth_user_dict["hash"] = hash

    """
  for emasterdata in emasterdata_server:
    item = {"type": emasterdata.value , "hash": "", "timing": 0, "tag_id": 0}
    hash.append(item)

  auth_user_dict["hash"] = hash
  """

    auth_user_text = SecureUtil.createhaders(auth_user_dict)
    csum_ans = SecureUtil.csum(auth_user_text + uuid)

    auth_user_data = 'request={0}&csum={1}&ott={2}'.format(
        auth_user_text, csum_ans, ott)

    return auth_user_data

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public enum LoginErrorCode : byte
    {
    RESPONSE_SUCCESS = 0x00,
    RESPONSE_FAILURE = 0x01,
    RESPONSE_CANCELLED = 0x02,
    RESPONSE_DISCONNECTED = 0x03,
    RESPONSE_FAILED_TO_CONNECT = 0x04,
    RESPONSE_CONNECTED = 0x05,
    RESPONSE_VERSION_MISMATCH = 0x06,

    CSTATUS_CONNECTING = 0x07,
    CSTATUS_NEGOTIATING_SECURITY = 0x08,
    CSTATUS_NEGOTIATION_COMPLETE = 0x09,
    CSTATUS_NEGOTIATION_FAILED = 0x0A,
    CSTATUS_AUTHENTICATING = 0x0B,

    AUTH_OK = 0x0C,
    AUTH_FAILED = 0x0D,
    AUTH_REJECT = 0x0E,
    AUTH_BAD_SERVER_PROOF = 0x0F,
    AUTH_UNAVAILABLE = 0x10,
    AUTH_SYSTEM_ERROR = 0x11,
    AUTH_BILLING_ERROR = 0x12,
    AUTH_BILLING_EXPIRED = 0x13,
    AUTH_VERSION_MISMATCH = 0x14,
    AUTH_UNKNOWN_ACCOUNT = 0x15,
    AUTH_INCORRECT_PASSWORD = 0x16,
    AUTH_SESSION_EXPIRED = 0x17,
    AUTH_SERVER_SHUTTING_DOWN = 0x18,
    AUTH_ALREADY_LOGGING_IN = 0x19,
    AUTH_LOGIN_SERVER_NOT_FOUND = 0x1A,
    AUTH_WAIT_QUEUE = 0x1B,
    AUTH_BANNED = 0x1C,
    AUTH_ALREADY_ONLINE = 0x1D,
    AUTH_NO_TIME = 0x1E,
    AUTH_DB_BUSY = 0x1F,
    AUTH_SUSPENDED = 0x20,
    AUTH_PARENTAL_CONTROL = 0x21,
    AUTH_LOCKED_ENFORCED = 0x02, /// Unsure

    REALM_LIST_IN_PROGRESS = 0x22,
    REALM_LIST_SUCCESS = 0x23,
    REALM_LIST_FAILED = 0x24,
    REALM_LIST_INVALID = 0x25,
    REALM_LIST_REALM_NOT_FOUND = 0x26,

    ACCOUNT_CREATE_IN_PROGRESS = 0x27,
    ACCOUNT_CREATE_SUCCESS = 0x28,
    ACCOUNT_CREATE_FAILED = 0x29,

    CHAR_LIST_RETRIEVING = 0x2A,
    CHAR_LIST_RETRIEVED = 0x2B,
    CHAR_LIST_FAILED = 0x2C,

    CHAR_CREATE_IN_PROGRESS = 0x2D,
    CHAR_CREATE_SUCCESS = 0x2E,
    CHAR_CREATE_ERROR = 0x2F,
    CHAR_CREATE_FAILED = 0x30,
    CHAR_CREATE_NAME_IN_USE = 0x31,
    CHAR_CREATE_DISABLED = 0x3A,
    CHAR_CREATE_PVP_TEAMS_VIOLATION = 0x33,
    CHAR_CREATE_SERVER_LIMIT = 0x34,
    CHAR_CREATE_ACCOUNT_LIMIT = 0x35,
    CHAR_CREATE_SERVER_QUEUE = 0x30,/// UNSURE
    CHAR_CREATE_ONLY_EXISTING = 0x30,/// UNSURE

    CHAR_DELETE_IN_PROGRESS = 0x38,
    CHAR_DELETE_SUCCESS = 0x39,
    CHAR_DELETE_FAILED = 0x3A,
    CHAR_DELETE_FAILED_LOCKED_FOR_TRANSFER = 0x3A,/// UNSURE
    CHAR_DELETE_FAILED_GUILD_LEADER = 0x3A,/// UNSURE

    CHAR_LOGIN_IN_PROGRESS = 0x3B,
    CHAR_LOGIN_SUCCESS = 0x3C,
    CHAR_LOGIN_NO_WORLD = 0x3D,
    CHAR_LOGIN_DUPLICATE_CHARACTER = 0x3E,
    CHAR_LOGIN_NO_INSTANCES = 0x3F,
    CHAR_LOGIN_FAILED = 0x40,
    CHAR_LOGIN_DISABLED = 0x41,
    CHAR_LOGIN_NO_CHARACTER = 0x42,
    CHAR_LOGIN_LOCKED_FOR_TRANSFER = 0x40, /// UNSURE
    CHAR_LOGIN_LOCKED_BY_BILLING = 0x40, /// UNSURE

    CHAR_NAME_SUCCESS = 0x50,
    CHAR_NAME_FAILURE = 0x4F,
    CHAR_NAME_NO_NAME = 0x43,
    CHAR_NAME_TOO_SHORT = 0x44,
    CHAR_NAME_TOO_LONG = 0x45,
    CHAR_NAME_INVALID_CHARACTER = 0x46,
    CHAR_NAME_MIXED_LANGUAGES = 0x47,
    CHAR_NAME_PROFANE = 0x48,
    CHAR_NAME_RESERVED = 0x49,
    CHAR_NAME_INVALID_APOSTROPHE = 0x4A,
    CHAR_NAME_MULTIPLE_APOSTROPHES = 0x4B,
    CHAR_NAME_THREE_CONSECUTIVE = 0x4C,
    CHAR_NAME_INVALID_SPACE = 0x4D,
    CHAR_NAME_CONSECUTIVE_SPACES = 0x4E,
    CHAR_NAME_RUSSIAN_CONSECUTIVE_SILENT_CHARACTERS = 0x4E,/// UNSURE
    CHAR_NAME_RUSSIAN_SILENT_CHARACTER_AT_BEGINNING_OR_END = 0x4E,/// UNSURE
    CHAR_NAME_DECLENSION_DOESNT_MATCH_BASE_NAME = 0x4E,/// UNSURE
}

public struct Character
    {
        public UInt64 GUID;
        public string Name;
        public byte Race;
        public byte Class;
        public byte RaceMask;
        public byte ClassMask;
        public byte Gender;
        public byte Skin;
        public byte Face;
        public byte HairStyle;
        public byte HairColour;
        public byte FacialHair;
        public uint GuildGuid;
        public byte OutfitID;
        public byte Level;
        public uint PetDisplayInfo;
        public uint PetLevel;
        public uint PetFamily;
        public uint Zone;
        public uint MapID;
        public float X;
        public float Y;
        public float Z;
        public float O;

    }

    public struct Realm
    {
        public byte Type;
        public byte Color;
        public byte NameLen;
        public string Name;
        public byte AddrLen;
        public string Address;
        public float Population;
        public byte NumChars;
        public byte Language;
        public byte Unk; // const: 1
        public int ID;
        public int wOnline;
    }

enum AuthResult
{
    WOW_SUCCESS = 0x00,
    WOW_FAIL_UNKNOWN0 = 0x01,                 ///< ? Unable to connect
    WOW_FAIL_UNKNOWN1 = 0x02,                 ///< ? Unable to connect
    WOW_FAIL_BANNED = 0x03,                 ///< This <game> account has been closed and is no longer available for use. Please go to <site>/banned.html for further information.
    WOW_FAIL_UNKNOWN_ACCOUNT = 0x04,                 ///< The information you have entered is not valid. Please check the spelling of the account name and password. If you need help in retrieving a lost or stolen password, see <site> for more information
    WOW_FAIL_INCORRECT_PASSWORD = 0x05,                 ///< The information you have entered is not valid. Please check the spelling of the account name and password. If you need help in retrieving a lost or stolen password, see <site> for more information
    // client reject next login attempts after this error, so in code used WOW_FAIL_UNKNOWN_ACCOUNT for both cases
    WOW_FAIL_ALREADY_ONLINE = 0x06,                 ///< This account is already logged into <game>. Please check the spelling and try again.
    WOW_FAIL_NO_TIME = 0x07,                 ///< You have used up your prepaid time for this account. Please purchase more to continue playing
    WOW_FAIL_DB_BUSY = 0x08,                 ///< Could not log in to <game> at this time. Please try again later.
    WOW_FAIL_VERSION_INVALID = 0x09,                 ///< Unable to validate game version. This may be caused by file corruption or interference of another program. Please visit <site> for more information and possible solutions to this issue.
    WOW_FAIL_VERSION_UPDATE = 0x0A,                 ///< Downloading
    WOW_FAIL_INVALID_SERVER = 0x0B,                 ///< Unable to connect
    WOW_FAIL_SUSPENDED = 0x0C,                 ///< This <game> account has been temporarily suspended. Please go to <site>/banned.html for further information
    WOW_FAIL_FAIL_NOACCESS = 0x0D,                 ///< Unable to connect
    WOW_SUCCESS_SURVEY = 0x0E,                 ///< Connected.
    WOW_FAIL_PARENTCONTROL = 0x0F,                 ///< Access to this account has been blocked by parental controls. Your settings may be changed in your account preferences at <site>
    WOW_FAIL_LOCKED_ENFORCED = 0x10,                 ///< You have applied a lock to your account. You can change your locked status by calling your account lock phone number.
    WOW_FAIL_TRIAL_ENDED = 0x11,                 ///< Your trial subscription has expired. Please visit <site> to upgrade your account.
    WOW_FAIL_USE_BATTLENET = 0x12                  ///< WOW_FAIL_OTHER This account is now attached to a Battle.net account. Please login with your Battle.net account email address and password.
    // WOW_FAIL_OVERMIND_CONVERTED
    // WOW_FAIL_ANTI_INDULGENCE
    // WOW_FAIL_EXPIRED
    // WOW_FAIL_NO_GAME_ACCOUNT
    // WOW_FAIL_BILLING_LOCK
    // WOW_FAIL_IGR_WITHOUT_BNET
    // WOW_FAIL_AA_LOCK
    // WOW_FAIL_UNLOCKABLE_LOCK
    // WOW_FAIL_MUST_USE_BNET
    // WOW_FAIL_OTHER
}

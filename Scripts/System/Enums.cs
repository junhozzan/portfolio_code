
public enum StatusType
{
    NONE = 0,
    INVINCIBLE = 1, // 무적
    FREEZING, // 빙결
    FLAME, // 화상
}

public enum BuffScriptType
{
    NONE,
    TICK_HEAL_MP, // 틱 mp 회복
    TICK_SPAWN_ASSIST_UNIT,
    TICK_SPAWN_SPECIAL_ASSIST_UNIT, // 특별 그림자 병사 소환
    KILL_TO_SPAWN_ASSIST_UNIT, // 적처치시 그림자 병사 소환
    START_SPAWN_ASSIST_UNIT, // 시작시 그림자 병사 소환
    STATUS, // 상태이상
    FREEZING_OPTION,
    ESCANOR, // 특정 시간 강해짐
    KILL_TO_DAMAGE_UP, // 처치시마다 대미지 상승
    ATTACK_TO_DAMAGE_UP, // 공격시마다 대미지 상승 
    ADD_STAT, // 스탯 지급
    ADD_SKILL, // 스킬 지급
    ATTACK_TO_SKILL, // 공격시 스킬 시전
    SKILL_BONUS, // 스킬 추가 능력
    ASSIST_UNIT_OPTION, // 그림자 병사 옵션
    ATTACK_TO_HEAL_HP, // 공격시 hp 회복
    MODE_PENALTY, // 모드 패널티
    SHOW_SPUM,
}

// 스킬 실행 타입
public enum SkillScriptType
{
    NONE = 0,
    ATTACK,
    PROJECTILE,
    PARABOLA,
    RANGE,
    TELEPORT,
    BUFF,
    JUMP,
}

public enum AchieveType
{
    NONE,
    ATTENDANCE,
    ENHANCE_ITEM,
    REROLL_ITEM,
    AWAKEN_ITEM,
    DISMANTLE_ITEM,
    UPGRADE_LAB,
    PURCHASE_STORE,
    SHOW_AD_COMPLETE,

    COUNT,
}

public enum MissionKeyType
{
    NONE,
    
    ENHANCE_ITEM, // 장비 업그레이드
    MODE_SCORE, // 모드 스코어 달성
    UPGRADE_LAB, // 연구소 업그레이드

    ATTENDANCE, // 출석
    AWAKEN_ITEM, // 장비 초월
    DISMANTLE_ITEM, // 장비 분해
    PURCHASE_STORE, // 아이템 구매

}

public enum MissionResetType
{
    NONE,
    DAY,
    WEEK,
}

public enum ItemType
{
    NONE,
    WEALTH,
    VIRTUAL, // 가상아이템

    WEAPON, // 무기
    SHIELD, // 방패
    ARMOR, // 갑옷
    HELMET, // 투구
    PANT, // 하의
    JEW, // 장신구
}

public enum ItemSetType
{
    NONE,
    // 위로 놓을수록 트리에서 아래 위치로 정렬
    CURSED, // 저주받은
    STURDY, // 튼튼한
}

public enum ItemFilterType
{
    EQUIP,

    STURDY,
    CURSED,

    WEAPON,
    ARMOR,
    JEW,

    GRADE_1,
    GRADE_2,
    GRADE_3,
    GRADE_4,
    GRADE_5,
    GRADE_6,
    GRADE_7,

    ALL,
}


public enum StoreResetType
{
    NONE,
    DAY,
    WEEK,
    INFINITY, // 리셋 없음
}

public enum DamageType
{
    NONE,
    NORMAL,
    HOLY,
    DARK,
}

public enum DamageByType
{
    NONE,
    HP,
    MAXHP,
}

public enum SkillType
{
    NONE,
    SKILL_TELEPORT,
    SKILL_JUMP,
    SKILL_BUFF,
    SKILL_ATTACK,
    DEFAULT_ATTACK,
    COUNT
}

// 발사 위치
public enum ShotPointType
{
    NONE,
    CASTER_POSITION,
    TARGET_POSITION,


    TARGET_POSITION_CUSTOM, // 타겟 위치 기준 커스텀 
}

public enum ModeType
{
    NONE,
    STAGE,
    DAMAGE,
    KILL,
    COUNT,
}

public enum PackItemType
{
    NONE,
    PACK,
    ITEM,
}

public enum AssistUnitOptionType
{
    NONE,
    STAT_SYNC, // 스탯 동기화
    COUNT,
}


public enum UnitAni
{
    NONE = 0,
    IDLE = 100,
    RUN = 200,
    ATTACK = 300,
    DEAD = 400,
    DEAD_2 = 500,
}

public enum Team
{
    NONE,
    ALLY,
    ENEMY,
}

public enum UnitGrade
{
    NONE,
    BOSS,
}

public enum GuideType
{
    NONE = 0,
    CREATE_ITEM = 1 << 0,
    MERGE_ITEM = 1 << 1,
}
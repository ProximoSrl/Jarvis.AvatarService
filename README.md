# Jarvis.AvatarService

Service for user avatar. Can use a custom provided .png or crearea a new with user's initials.


##API

    /api/avatar/user_id?size=pixels&name=First+Last

Examples

    /api/avatar/user_3?size=80&name=Super+Hero
    /api/avatar/user_1?size=40&name=John+Doe

##Configuration
All generated avatars are cached in the App_Data\size\ folder.

Personal user images can be provided in .png format in the AppData\Avatars folder.
 
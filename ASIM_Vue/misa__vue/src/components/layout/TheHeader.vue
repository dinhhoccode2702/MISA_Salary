<template>
  <header class="the-header flex justify-between align-center">
    <div class="header-left flex align-center">
      <div
        class="icon-app-selector"
        role="button"
        aria-label="Danh mục ứng dụng"
        data-tooltip="Danh mục ứng dụng"
        data-tooltip-position="bottom"
      ></div>
      <div class="brand flex align-center">
        <img src="../../assets/img/Logo_30-4.a5741a5.svg" alt="MISA Logo" class="logo" />
        <span class="app-name">Tiền lương</span>
      </div>
      <div class="header-search-wrapper">
        <div class="ms-input flex w-full">
          <div class="ms-icon-default flex items-center justify-center">
            <i class="ms-icon notranslate icon-scale mi-search"></i>
          </div>
          <input maxlength="255" placeholder="Tìm kiếm" tabindex="1" autocomplete="on" type="text" class="ms-input-item">
        </div>
      </div>
    </div>

    <div class="header-right flex align-center">
      <div id="AMIS_UserControlPanel" class="flex align-center">
        <div id="AMIS_Cart" class="utility-icon" role="button" aria-label="Giỏ hàng" data-tooltip="Giỏ hàng" data-tooltip-position="bottom">
          <div class="icon-cart"></div>
        </div>
        <div id="AMIS_BotAI" class="utility-icon" role="button" aria-label="AI hỗ trợ" data-tooltip="AI hỗ trợ" data-tooltip-position="bottom">
          <div class="icon-bot-ai"></div>
        </div>
        <div id="AMIS_newFeature" class="utility-icon" role="button" aria-label="Tính năng mới" data-tooltip="Tính năng mới" data-tooltip-position="bottom">
          <div class="icon-newFeature"></div>
        </div>
        <div id="AMIS_MobileAds" class="utility-icon" role="button" aria-label="Tải ứng dụng" data-tooltip="Tải ứng dụng" data-tooltip-position="bottom">
          <div class="icon-mobile-ads"></div>
        </div>
        <div id="AMIS_InviteBox" class="utility-icon" role="button" aria-label="Mời đồng nghiệp" data-tooltip="Mời đồng nghiệp" data-tooltip-position="bottom">
          <div class="icon-invite"></div>
        </div>
        <div id="AMIS_MessageBox" class="utility-icon" role="button" aria-label="Trao đổi" data-tooltip="Trao đổi" data-tooltip-position="bottom">
          <div class="icon-message"></div>
        </div>
        <div id="AMIS_Notification" class="utility-icon" role="button" aria-label="Thông báo" data-tooltip="Thông báo" data-tooltip-position="bottom">
          <div class="icon-notify"></div>
          <span class="badge">2</span>
        </div>
        <div id="MISA_ButtonHelp" class="utility-icon" role="button" aria-label="Trợ giúp" data-tooltip="Trợ giúp" data-tooltip-position="bottom">
          <div class="icon-help"></div>
        </div>
        <div id="MISA_MoreMenu" class="utility-icon" role="button" aria-label="Tiện ích khác" data-tooltip="Tiện ích khác" data-tooltip-position="bottom">
          <div class="icon-more"></div>
        </div>
      </div>

      <div id="User_Menu_Account" class="user-info flex align-center">
        <img src="https://ui-avatars.com/api/?name=Admin&background=2ca01c&color=fff" alt="Avatar" class="avatar" />
      </div>
    </div>
  </header>
</template>

<script setup>
// Header logic here
</script>

<style scoped>
.the-header {
  height: var(--header-height);
  background-color: var(--header-bg);
  padding: 0 16px;
  border-bottom: 1px solid var(--border-color);
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  min-width: 1366px;
  z-index: 1000;
}

/* Left Header */
.logo {
  width: auto !important;
  height: 40px !important;
}

.app-name {
  margin-left: 12px;
  font-size: 18px;
  color: #000;
  font-weight: 700;
  margin-left: 6px;
  margin-right: 24px;
}

.header-search-wrapper {
  width: 300px;
  margin: 4px;
}

.ms-input {
  height: 32px;
  border: none;
  border-radius: 4px;
  background-color: #f6f6f6;
  min-height: 28px;
  align-items: center;
  overflow: hidden;
  display: flex;
  width: 100%;
}

.ms-icon-default {
  width: 32px;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mi-search {
  background-image: url('../../assets/img/Icon.c487640.svg');
  background-position: 0px 0px;
  width: 20px;
  height: 20px;
  display: inline-block;
}


.ms-input-item {
  flex: 1;
  background: transparent;
  border: none;
  font-size: 13px;
  width: 100%;
  padding: 0 8px;
  outline: none;
}

/* Right Header */
.utility-icon {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  cursor: pointer;
  position: relative;
  margin: 0 2px;
}

.utility-icon > div {
  flex-shrink: 0;
  margin: 0 !important;
  position: static;
}

.utility-icon:hover {
  background-color: #f5f5f5;
}

.badge {
  position: absolute;
  top: 4px;
  right: 4px;
  background-color: var(--color-error);
  color: white;
  font-size: 10px;
  padding: 1px 4px;
  border-radius: 10px;
  border: 1px solid white;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  margin-left: 12px;
  margin-right: 8px;
}

/* Icon Sprites */
.icon-app-selector {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  position: relative;
  cursor: pointer;
}

.icon-app-selector:hover {
  background-color: #f5f5f5;
}

.icon-app-selector::before {
  content: '';
  position: absolute;
  inset: 6px;
  background-image: url('../../assets/img/Icon.c487640.svg');
  background-position: -80px -80px;
}

.icon-cart{
  background-image: url('https://cdnstore.misacdn.net/lib/static/cs-icon-store.svg');
  background-position: 20px 20px;
  width: 20px;
  height: 20px;
}

.icon-bot-ai {
  background-image: url('../../assets/img/payroll_agent.86d4797.png');
  background-size: cover;
  height: 24px;
  width: 24px;
}

.icon-newFeature {
    width: 24px;
    height: 24px;
    background: url(https://amisplatform.misacdn.net/apps/payroll/static/img/Icon.c487640.svg) -336px -593px no-repeat;
}

.icon-mobile-ads {
  background-image: url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4NCjxwYXRoIGQ9Ik01IDQuMjIyMjJDNSAzLjYzMjg1IDUuMjI4MjcgMy4wNjc2MiA1LjYzNDYgMi42NTA4N0M2LjA0MDkzIDIuMjM0MTMgNi41OTIwMyAyIDcuMTY2NjcgMkgxNS44MzMzQzE2LjQwOCAyIDE2Ljk1OTEgMi4yMzQxMyAxNy4zNjU0IDIuNjUwODdDMTcuNzcxNyAzLjA2NzYyIDE4IDMuNjMyODUgMTggNC4yMjIyMlYxOS43Nzc4QzE4IDIwLjM2NzEgMTcuNzcxNyAyMC45MzI0IDE3LjM2NTQgMjEuMzQ5MUMxNi45NTkxIDIxLjc2NTkgMTYuNDA4IDIyIDE1LjgzMzMgMjJINy4xNjY2N0M2LjU5MjAzIDIyIDYuMDQwOTMgMjEuNzY1OSA1LjYzNDYgMjEuMzQ5MUM1LjIyODI3IDIwLjkzMjQgNSAyMC4zNjcxIDUgMTkuNzc3OFY0LjIyMjIyWiIgZmlsbD0iI0U4RjVGRiIvPg0KPHBhdGggZD0iTTEwLjQxNjcgMy4xMTExMUgxMi41ODMzWiIgZmlsbD0iI0U4RjVGRiIvPg0KPHBhdGggZD0iTTExLjUgMTcuNTU1NlYxNy41NjY3WiIgZmlsbD0iI0U4RjVGRiIvPg0KPHBhdGggZD0iTTEwLjQxNjcgMy4xMTExMUgxMi41ODMzTTExLjUgMTcuNTU1NlYxNy41NjY3TTUgNC4yMjIyMkM1IDMuNjMyODUgNS4yMjgyNyAzLjA2NzYyIDUuNjM0NiAyLjY1MDg3QzYuMDQwOTMgMi4yMzQxMyA2LjU5MjAzIDIgNy4xNjY2NyAySDE1LjgzMzNDMTYuNDA4IDIgMTYuOTU5MSAyLjIzNDEzIDE3LjM2NTQgMi42NTA4N0MxNy43NzE3IDMuMDY3NjIgMTggMy42MzI4NSAxOCA0LjIyMjIyVjE5Ljc3NzhDMTggMjAuMzY3MSAxNy43NzE3IDIwLjkzMjQgMTcuMzY1NCAyMS4zNDkxQzE2Ljk1OTEgMjEuNzY1OSAxNi40MDggMjIgMTUuODMzMyAyMkg3LjE2NjY3QzYuNTkyMDMgMjIgNi4wNDA5MyAyMS43NjU5IDUuNjM0NiAyMS4zNDkxQzUuMjI4MjcgMjAuOTMyNCA1IDIwLjM2NzEgNSAxOS43Nzc4VjQuMjIyMjJaIiBzdHJva2U9InVybCgjcGFpbnQwX2xpbmVhcl8xMDE5XzE4Njk4KSIgc3Ryb2tlLXdpZHRoPSIyLjEiIHN0cm9rZS1saW5lY2FwPSJyb3VuZCIgc3Ryb2tlLWxpbmVqb2luPSJyb3VuZCIvPg0KPGRlZnM+DQo8bGluZWFyR3JhZGllbnQgaWQ9InBhaW50MF9saW5lYXJfMTAxOV8xODY5OCIgeDE9IjUuMTM2OTciIHkxPSIwLjc3MDQ5MiIgeDI9IjI0Ljk2NDQiIHkyPSIxOC42NDIyIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+DQo8c3RvcCBzdG9wLWNvbG9yPSIjMDdCOUZGIi8+DQo8c3RvcCBvZmZzZXQ9IjAuNTMxNjY2IiBzdG9wLWNvbG9yPSIjNTE0OEZGIi8+DQo8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiM4ODAwRkYiLz4NCjwvbGluZWFyR3JhZGllbnQ+DQo8L2RlZnM+DQo8L3N2Zz4NCg==');
  width: 32px;
  height: 32px;
  border-radius: 16px;
  background-position: center;
  background-size: 24px;
  background-repeat: no-repeat;
}

.icon-invite {
  width: 32px;
    height: 32px;
    margin-top: 0 !important;
    border-radius: 16px;
    background-position: center;
    background-size: 24px;
    background-repeat: no-repeat;
    overflow: unset;
    cursor: pointer;
  position: relative;
  background-image: url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iMTEuNSIgY3k9IjUuNSIgcj0iMy41IiBzdHJva2U9IiM3NTc1NzUiIHN0cm9rZS13aWR0aD0iMiIvPgo8cGF0aCBkPSJNMy41MDYyOSAxNS43NDEzTDMuMTA2NTIgMTYuMjk2NUMxLjM5MTk2IDE4LjY3NzggMy4wOTM2OCAyMiA2LjAyODA0IDIySDE1Ljk3MkMxOC45MDYzIDIyIDIwLjYwOCAxOC42Nzc4IDE4Ljg5MzUgMTYuMjk2NUwxOC40OTM3IDE1Ljc0MTNDMTYuODAyNCAxMy4zOTIyIDE0LjA4NDUgMTIgMTEuMTg5OSAxMkgxMC44MTAxQzcuOTE1NTMgMTIgNS4xOTc2MSAxMy4zOTIyIDMuNTA2MjkgMTUuNzQxM1oiIHN0cm9rZT0iIzc1NzU3NSIgc3Ryb2tlLXdpZHRoPSIyIiBzdHJva2UtbGluZWNhcD0icm91bmQiLz4KPHBhdGggZD0iTTIxIDVWN1Y5IiBzdHJva2U9IiMyQ0EwMUMiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIi8+CjxwYXRoIGQ9Ik0xOSA3SDIxSDIzIiBzdHJva2U9IiMyQ0EwMUMiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIi8+Cjwvc3ZnPgo=');
}

.icon-message {
background-image: url('data:image/svg+xml;charset=utf-8;base64,PHN2ZyB3aWR0aD0nMjQnIGhlaWdodD0nMjQnIHZpZXdCb3g9JzAgMCAyNCAyNCcgZmlsbD0nbm9uZScgeG1sbnM9J2h0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnJz48cGF0aCBkPSdNMTIgM0M3LjAyOTQ0IDMgMyA3LjAyOTQ1IDMgMTJDMyAxMy43NDk5IDMuNDk5OTIgMTUuMzgyNyA0LjM2Mzk2IDE2Ljc2NDVMMyAyMUw3LjIzNTQ1IDE5LjYzNkM4LjYxNzMgMjAuNTAwMSAxMC4yNTAxIDIxIDEyIDIxQzE2Ljk3MDYgMjEgMjEgMTYuOTcwNiAyMSAxMkMyMSA3LjAyOTQ1IDE2Ljk3MDYgMyAxMiAzWicgZmlsbD0ndXJsKCNwYWludDBfbGluZWFyXzUwMjRfMzczNjkpJyBzdHJva2U9J3VybCgjcGFpbnQxX2xpbmVhcl81MDI0XzM3MzY5KScgc3Ryb2tlLW1pdGVybGltaXQ9JzEwJyBzdHJva2UtbGluZWNhcD0ncm91bmQnIHN0cm9rZS1saW5lam9pbj0ncm91bmQnLz48cGF0aCBkPSdNMTMuMDk4NiAxMi4wMDAxQzEzLjA5ODYgMTIuNjA2OSAxMi42MDY4IDEzLjA5ODggMTIgMTMuMDk4OEMxMS4zOTMyIDEzLjA5ODggMTAuOTAxNCAxMi42MDY5IDEwLjkwMTQgMTIuMDAwMUMxMC45MDE0IDExLjM5MzQgMTEuMzkzMiAxMC45MDE1IDEyIDEwLjkwMTVDMTIuNjA2OCAxMC45MDE1IDEzLjA5ODYgMTEuMzkzNCAxMy4wOTg2IDEyLjAwMDFaJyBmaWxsPSd3aGl0ZScvPjxwYXRoIGQ9J00xNy40OTMyIDEyLjAwMDFDMTcuNDkzMiAxMi42MDY5IDE3LjAwMTMgMTMuMDk4OCAxNi4zOTQ1IDEzLjA5ODhDMTUuNzg3OCAxMy4wOTg4IDE1LjI5NTkgMTIuNjA2OSAxNS4yOTU5IDEyLjAwMDFDMTUuMjk1OSAxMS4zOTM0IDE1Ljc4NzggMTAuOTAxNSAxNi4zOTQ1IDEwLjkwMTVDMTcuMDAxMyAxMC45MDE1IDE3LjQ5MzIgMTEuMzkzNCAxNy40OTMyIDEyLjAwMDFaJyBmaWxsPSd3aGl0ZScvPjxwYXRoIGQ9J004LjcwNDEgMTIuMDAwMUM4LjcwNDEgMTIuNjA2OSA4LjIxMjIyIDEzLjA5ODggNy42MDU0NyAxMy4wOTg4QzYuOTk4NzIgMTMuMDk4OCA2LjUwNjg0IDEyLjYwNjkgNi41MDY4NCAxMi4wMDAxQzYuNTA2ODQgMTEuMzkzNCA2Ljk5ODcyIDEwLjkwMTUgNy42MDU0NyAxMC45MDE1QzguMjEyMjIgMTAuOTAxNSA4LjcwNDEgMTEuMzkzNCA4LjcwNDEgMTIuMDAwMVonIGZpbGw9J3doaXRlJy8+PGRlZnM+PGxpbmVhckdyYWRpZW50IGlkPSdwYWludDBfbGluZWFyXzUwMjRfMzczNjknIHgxPScxMicgeTE9JzMnIHgyPScxMicgeTI9JzIxJyBncmFkaWVudFVuaXRzPSd1c2VyU3BhY2VPblVzZSc+PHN0b3Agc3RvcC1jb2xvcj0nIzVCN0NGRicvPjxzdG9wIG9mZnNldD0nMScgc3RvcC1jb2xvcj0nIzIxNEJGNScvPjwvbGluZWFyR3JhZGllbnQ+PGxpbmVhckdyYWRpZW50IGlkPSdwYWludDFfbGluZWFyXzUwMjRfMzczNjknIHgxPScxMicgeTE9JzMnIHgyPScxMicgeTI9JzIxJyBncmFkaWVudFVuaXRzPSd1c2VyU3BhY2VPblVzZSc+PHN0b3Agc3RvcC1jb2xvcj0nIzVCN0NGRicvPjxzdG9wIG9mZnNldD0nMScgc3RvcC1jb2xvcj0nIzIxNEJGNScvPjwvbGluZWFyR3JhZGllbnQ+PC9kZWZzPjwvc3ZnPg==');
width: 24px;
height: 24px;
}

.icon-notify { 
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCI+DQogIDxkZWZzPg0KICAgIDxjbGlwUGF0aCBpZD0iY2xpcC1wYXRoIj4NCiAgICAgIDxyZWN0IGlkPSJSZWN0YW5nbGVfMjIyMTUiIGRhdGEtbmFtZT0iUmVjdGFuZ2xlIDIyMjE1IiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIGZpbGw9IiNmZmYiLz4NCiAgICA8L2NsaXBQYXRoPg0KICA8L2RlZnM+DQogIDxnIGlkPSJHcm91cF80NjI4NyIgZGF0YS1uYW1lPSJHcm91cCA0NjI4NyIgY2xpcC1wYXRoPSJ1cmwoI2NsaXAtcGF0aCkiPg0KICAgIDxwYXRoIGlkPSJQYXRoXzM2MjU3IiBkYXRhLW5hbWU9IlBhdGggMzYyNTciIGQ9Ik0xOCwxMS42VjkuM2E2LjE4NCw2LjE4NCwwLDAsMC01Ljg0Mi02LjI3NkE2LDYsMCwwLDAsNiw5LjAyNFYxMS42YzAsMi44MjgtMi41NzEsMy41MTQtMi41NzEsNS4xNDJDMy40MjksMTguMiw2Ljc3MSwxOS4zMSwxMiwxOS4zMXM4LjU3MS0xLjExNCw4LjU3MS0yLjU3MkMyMC41NzEsMTUuMTEsMTgsMTQuNDI0LDE4LDExLjZaIiBmaWxsPSJub25lIiBzdHJva2U9IiM2ZTczN2EiIHN0cm9rZS1saW5lY2FwPSJzcXVhcmUiIHN0cm9rZS1taXRlcmxpbWl0PSIxMCIgc3Ryb2tlLXdpZHRoPSIyIi8+DQogICAgPHBhdGggaWQ9IlBhdGhfMzYyNTgiIGRhdGEtbmFtZT0iUGF0aCAzNjI1OCIgZD0iTTEyLDIxLjIyOHEtMS4yNSwwLTIuNS0uMDg3YTIuNjE4LDIuNjE4LDAsMCwwLDUsMFExMy4zMjUsMjEuMjI2LDEyLDIxLjIyOFoiIGZpbGw9IiM2ZTczN2EiLz4NCiAgPC9nPg0KPC9zdmc+DQo=');
      border-radius: 50%;
    cursor: pointer;
    width: 32px;
    height: 32px;
    font-size: 14px;
    background-repeat: no-repeat;
    background-position: center;
}

.icon-help { 
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCI+DQogIDxkZWZzPg0KICAgIDxjbGlwUGF0aCBpZD0iY2xpcC1wYXRoIj4NCiAgICAgIDxyZWN0IGlkPSJSZWN0YW5nbGVfMjgxNTkiIGRhdGEtbmFtZT0iUmVjdGFuZ2xlIDI4MTU5IiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHRyYW5zZm9ybT0idHJhbnNsYXRlKDQ0IDc5KSIgZmlsbD0iI2ZmZiIvPg0KICAgIDwvY2xpcFBhdGg+DQogIDwvZGVmcz4NCiAgPGcgaWQ9Ikdyb3VwXzgyNzg0IiBkYXRhLW5hbWU9Ikdyb3VwIDgyNzg0IiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtNDY5OSAtMTcwKSI+DQogICAgPGcgaWQ9Ikdyb3VwXzgyNzc4IiBkYXRhLW5hbWU9Ikdyb3VwIDgyNzc4IiB0cmFuc2Zvcm09InRyYW5zbGF0ZSg0NjU1IDkxKSIgY2xpcC1wYXRoPSJ1cmwoI2NsaXAtcGF0aCkiPg0KICAgICAgPHBhdGggaWQ9IlBhdGhfODkwNjQiIGRhdGEtbmFtZT0iUGF0aCA4OTA2NCIgZD0iTTUzLjQsODguMzE1YTIuNjg1LDIuNjg1LDAsMCwxLDUuMjE4LjlDNTguNjEzLDkxLDU2LDkxLjksNTYsOTEuOXYuOSIgZmlsbD0ibm9uZSIgc3Ryb2tlPSIjNmU3MzdhIiBzdHJva2UtbGluZWNhcD0icm91bmQiIHN0cm9rZS1saW5lam9pbj0icm91bmQiIHN0cm9rZS13aWR0aD0iMS44Ii8+DQogICAgICA8cGF0aCBpZD0iUGF0aF84OTA2NSIgZGF0YS1uYW1lPSJQYXRoIDg5MDY1IiBkPSJNNTYsMTAwYTksOSwwLDEsMC05LTlBOSw5LDAsMCwwLDU2LDEwMFoiIGZpbGw9Im5vbmUiIHN0cm9rZT0iIzZlNzM3YSIgc3Ryb2tlLWxpbmVjYXA9InNxdWFyZSIgc3Ryb2tlLW1pdGVybGltaXQ9IjEwIiBzdHJva2Utd2lkdGg9IjIiLz4NCiAgICAgIDxwYXRoIGlkPSJQYXRoXzg5MDY2IiBkYXRhLW5hbWU9IlBhdGggODkwNjYiIGQ9Ik01Niw5N2ExLDEsMCwxLDAtMS0xQTEsMSwwLDAsMCw1Niw5N1oiIGZpbGw9IiM2ZTczN2EiLz4NCiAgICA8L2c+DQogIDwvZz4NCjwvc3ZnPg0K');
    width: 32px;
    height: 32px;
    border-radius: 16px;
    background-position: center;
    background-size: 24px;
    background-repeat: no-repeat;
    overflow: unset;
    cursor: pointer;
}

.icon-more {
    width: 22px;
    height: 22px;
    background-image: url('../../assets/img/Icon.c487640.svg') ;
    background-position: -2px -122px;
    background-repeat: no-repeat;
}
</style>


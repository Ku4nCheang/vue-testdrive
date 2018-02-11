<template>
    <div class="pure-menu pure-p doc">
        <span class="pure-menu-heading">Template</span>

        <ul class="pure-menu-list">
            <li class="pure-menu-item">
                <router-link to="/" class="pure-menu-link">home</router-link>
            </li>
            <li class="pure-menu-item">
                <router-link to="/fetchdata" class="pure-menu-link">fetchdata</router-link>
            </li>
        </ul>
    </div>
</template>

<style>
    .pure-menu.doc {
        color: white;
        background: black;
    }
</style>


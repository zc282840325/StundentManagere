import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../Login.vue'

Vue.use(VueRouter)

const routes = [{
		path: '/',
		name: 'Login',
		component: Login
	},
	{
		path: '/home',
		name: 'Home',
		// route level code-splitting
		// this generates a separate chunk (about.[hash].js) for this route
		// which is lazy-loaded when the route is visited.
		component: () => import( /* webpackChunkName: "about" */ '../views/Home.vue'),
		children: [
			// 当 /user/:id 匹配成功，
			// UserHome 会被渲染在 User 的 <router-view> 中
			{
				path: '/about',
				component: () => import( /* webpackChunkName: "about" */ '../views/About.vue')
			},
			{
				path: '/test1',
				component: () => import( /* webpackChunkName: "about" */ '../views/test1.vue')
			},
			{
				path: '/test2',
				component: () => import( /* webpackChunkName: "about" */ '../views/test2.vue')
			},
			{
				path: '/UserAU',
				component: () => import( /* webpackChunkName: "about" */ '../views/User/UserAU.vue')
			},
			{
				path: '/UserAU/:id',
				component: () => import( /* webpackChunkName: "about" */ '../views/User/UserAU.vue'),
				props: true
			}, {
				path: '/UserIndex',
				component: () => import( /* webpackChunkName: "about" */ '../views/User/UserIndex.vue')
			}, {
				path: '/RoleIndex',
				component: () => import( /* webpackChunkName: "about" */ '../views/Role/RoleIndex.vue')
			},
			{
				path: '/RoleAU',
				component: () => import( /* webpackChunkName: "about" */ '../views/Role/RoleAU.vue')
			},
			{
				path: '/RoleAU/:id',
				component: () => import( /* webpackChunkName: "about" */ '../views/Role/RoleAU.vue')
			},
			{
				path: '/PermissionIndex',
				component: () => import( /* webpackChunkName: "about" */ '../views/Permission/PermissionIndex.vue')
			},
			{
				path: '/PermissionAU',
				component: () => import( /* webpackChunkName: "about" */ '../views/Permission/PermissionAU.vue')
			},
			{
				path: '/PermissionAU/:id',
				component: () => import( /* webpackChunkName: "about" */ '../views/Permission/PermissionAU.vue')
			},
			{
				path: '/UserRole',
				component: () => import( /* webpackChunkName: "about" */ '../views/User/UserRole.vue')
			},
			{
				path: '/UserRole/:id',
				component: () => import( /* webpackChunkName: "about" */ '../views/User/UserRole.vue')
			}
		]
	}

]

const router = new VueRouter({
	mode: 'history',
	base: process.env.BASE_URL,
	routes
})
//路由守卫
router.beforeEach((to, from, next) => {
	if(to.path=="/")
	{ 
		if(localStorage.getItem("accessToken"))	
		{ 
			　router.push({ path: '/UserIndex', });;
		}
	}
    Vue.prototype.axios.get('/Login/Verification?token='+localStorage.getItem("accessToken")).then((response) => {
			console.log(response.data.success);
			if(!response.data.success)
			{ 
				Vue.prototype.axios.get('Login/RefreshToken?token='+localStorage.getItem("accessToken")).then((response) => {
			    console.log('RefreshToken'+response.data);
				});
			}
	});

   // const data2=Vue.prototype.axios.get('Login/RefreshToken?token='+localStorage.getItem("accessToken"));
    next();
})

export default router

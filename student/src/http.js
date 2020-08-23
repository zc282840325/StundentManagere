import axios from 'axios'
import Vue from 'vue'
import router from './router/index'

const http = axios.create({
    baseURL: "http://localhost:5000/api",
})

//请求
http.interceptors.request.use((config) => {
    if (localStorage.accessToken) {
        config.headers.Authorization = 'Bearer ' + (localStorage.accessToken || '')
    }
    return config;
}, (err) => {
    return Promise.reject(err);
});

//响应拦截器
http.interceptors.response.use(res => {
    return res
}, err => {
	debugger;
    // 提示错误信息
    if (err.response.data.message) {
        Vue.prototype.$message({
            type: "error",
            message: err.response.data.message
        })
    }
	
    if (err.response.status == 401 ) {
		Vue.prototype.$message({
		    type: "error",
		    message: "权限不够"
		})
        router.push('/')
    }
	if(err.response.status == 500)
	{ 
		Vue.prototype.$message({
		    type: "error",
		    message: "服务器出错"
		})
		router.push('/')
	}

    return Promise.reject(err);
})

export default http
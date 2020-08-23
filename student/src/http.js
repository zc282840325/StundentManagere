import axios from 'axios'
import Vue from 'vue'
import router from './router/index'

const http = axios.create({
    baseURL: "http://localhost:5000/api",
})

//����
http.interceptors.request.use((config) => {
    if (localStorage.accessToken) {
        config.headers.Authorization = 'Bearer ' + (localStorage.accessToken || '')
    }
    return config;
}, (err) => {
    return Promise.reject(err);
});

//��Ӧ������
http.interceptors.response.use(res => {
    return res
}, err => {
	debugger;
    // ��ʾ������Ϣ
    if (err.response.data.message) {
        Vue.prototype.$message({
            type: "error",
            message: err.response.data.message
        })
    }
	
    if (err.response.status == 401 ) {
		Vue.prototype.$message({
		    type: "error",
		    message: "Ȩ�޲���"
		})
        router.push('/')
    }
	if(err.response.status == 500)
	{ 
		Vue.prototype.$message({
		    type: "error",
		    message: "����������"
		})
		router.push('/')
	}

    return Promise.reject(err);
})

export default http
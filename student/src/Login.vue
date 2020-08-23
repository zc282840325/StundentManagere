<template>
	<div>
		 <el-form ref="loginForm" :model="form" :rules="rules" label-width="80px" class="login-box">
		      <h3 class="login-title">欢迎登录</h3>
		      <el-form-item label="账号" prop="username">
		        <el-input type="text" placeholder="请输入账号" v-model="form.UserName"/>
		      </el-form-item>
		      <el-form-item label="密码" prop="password">
		        <el-input type="password" placeholder="请输入密码" v-model="form.UserPwd"/>
		      </el-form-item>
		      <el-form-item>
		        <el-button type="primary" v-on:click="Login('loginForm')">登录</el-button>
		      </el-form-item>
		    </el-form>
		
		    <el-dialog
		      title="温馨提示"
		      :visible.sync="dialogVisible"
		      width="30%"
		      :before-close="handleClose">
		      <span>请输入账号和密码</span>
		      <span slot="footer" class="dialog-footer">
		        <el-button type="primary" @click="dialogVisible = false">确 定</el-button>
		      </span>
		    </el-dialog>
		
	</div>
</template>

<script>
export default {
	data() {
		return {
			form: {
			          UserName: '',
			          UserPwd: ''
			        },
			rules: {
			UserName: [
					            {required: true, message: '账号不可为空', trigger: 'blur'}
					          ],
			UserPwd: [
					            {required: true, message: '密码不可为空', trigger: 'blur'}
					          ]
					        },
							 // 对话框显示和隐藏
			dialogVisible: false
		};
	},
	methods: {
		
		async Login() {
			console.log("login");
			const data = await this.axios.post('/Login',this.form);

			if (data.status == '200') {
				this.$message({
					message: '恭喜你，这是一条成功消息',
					type: 'success',
					duration: 1000
				});
				localStorage.setItem("username", data.data.username);
	            localStorage.setItem("accessToken", data.data.accessToken.token);
			
				this.$router.push({ path: '/UserIndex' });
			} else {
			this.$message({
					message: '账户密码不正确！',
					type: 'error',
					duration: 1000
				});
			}
		},
		handleClose(){
			
		}
	}
};
</script>

<style  scoped>
  .login-box {
    border: 1px solid #DCDFE6;
    width: 350px;
    margin: 180px auto;
    padding: 35px 35px 15px 35px;
    border-radius: 5px;
    -webkit-border-radius: 5px;
    -moz-border-radius: 5px;
    box-shadow: 0 0 25px #909399;
  }

  .login-title {
    text-align: center;
    margin: 0 auto 40px auto;
    color: #303133;
  }
</style>

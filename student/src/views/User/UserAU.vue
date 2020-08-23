<template>
	<div>
	
		<el-form ref="form" :model="form" label-width="80px" id="form1">
				<h3>{{this.id?'修改用户':'新增用户'}}</h3>
			<el-form-item label="用户名"><el-input v-model="form.UserName"></el-input></el-form-item>
			<el-form-item label="密码"><el-input v-model="form.UserPwd"></el-input></el-form-item>
			<el-form-item label="地区"><el-input v-model="form.Address"></el-input></el-form-item>
			<el-form-item>
				<el-button type="primary" @click="onSubmit">{{this.id?'保存':'创建'}}</el-button>
				<router-link to="/UserIndex"><el-button>取消</el-button></router-link>
			</el-form-item>
		</el-form>
	</div>
</template>

<script>
export default {
	data() {
		return {
			form: {
				UserName: '',
				UserPwd: '',
				Address: ''
			}
		};
	},
	props: ['id'],
	methods: {
		async onSubmit() {
			if (!this.id) {
				const data= await this.axios.post('/Users', this.form);
				if (data != null) {
					this.$message({
						message: '恭喜你，添加消息成功',
						type: 'success',
						duration: 1000
					});

					this.$router.push({ path: '/test1' });
				}
			} else {
				const data = await this.axios.put('/Users/' + this.id, this.form);
				if (data != null) {
					this.$message({
						message: '恭喜你，添加消息成功',
						type: 'success',
						duration: 1000
					});

					this.$router.push({ path: '/UserIndex' });
				}
				console.log('修改');
			}
		},
		getUser() {
			this.axios.get('/Users/' + this.id).then(response => {
				debugger;
				this.form = response.data.response;
			});
		}
	},
	mounted() {
		if (!this.id) {
			console.log('id:'+this.id);
		} else {
			console.log('id:'+this.id);
			this.getUser();
		}
	}
};
</script>

<style>
	#form1{
		width: 35%;
	}
</style>

<template>
	<div>
		<h3>{{this.id?'修改权限':'新增权限'}}</h3>
		<el-form ref="form" :model="form" label-width="80px">
			<el-form-item label="路由地址"><el-input v-model="form.address"></el-input></el-form-item>
			<el-form-item>
				<el-button type="primary" @click="onSubmit">{{this.id?'保存':'创建'}}</el-button>
				<router-link to="/PermissionIndex"><el-button>取消</el-button></router-link>
			</el-form-item>
		</el-form>
	</div>
</template>

<script>
export default {
	data() {
		return {
			form: {
				address: ''
			}
		};
	},
	props: ['id'],
	methods: {
		async onSubmit() {
			if (!this.id) {
				const data= await this.axios.post('/Permissions', this.form);
				if (data != null) {
					this.$message({
						message: '恭喜你，添加消息成功',
						type: 'success',
						duration: 1000
					});

					this.$router.push({ path: '/PermissionIndex' });
				}
			} else {
				const data = await this.axios.put('/Permissions/' + this.id, this.form);
				if (data != null) {
					this.$message({
						message: '恭喜你，添加消息成功',
						type: 'success',
						duration: 1000
					});

					this.$router.push({ path: '/PermissionIndex' });
				}
				console.log('修改');
			}
		},
		getUser() {
			this.axios.get('/Permissions/' + this.id).then(response => {
				console.log(response.data);
				this.form = response.data;
			});
		}
	},
	mounted() {
		if (!this.id) {
		} else {
			this.getUser();
		}
	}
};
</script>

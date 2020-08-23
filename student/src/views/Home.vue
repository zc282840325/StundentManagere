<template>
	<div>
		<el-container style="height: 500px; border: 1px solid #eee">
		  <el-aside width="200px" style="background-color: rgb(238, 241, 246)">
		    <el-menu :default-openeds="['1']" unique-opened :default-active="$route.path" router="">
		      <el-submenu index="1">
		        <template slot="title"><i class="el-icon-message"></i>后台管理</template>
		        <el-menu-item-group>
		          <el-menu-item index="/UserIndex">用户管理
				  </el-menu-item>
				   <el-menu-item index="/RoleIndex">角色管理</el-menu-item>
				      <el-menu-item index="/PermissionIndex">权限管理</el-menu-item>
		        </el-menu-item-group>
		      </el-submenu>
		      <el-submenu index="2">
		        <template slot="title"><i class="el-icon-menu"></i>导航二</template>
		        <el-menu-item-group>
		          <template slot="title">分组一</template>
		          <el-menu-item index="2-1">选项1</el-menu-item>
		          <el-menu-item index="2-2">选项2</el-menu-item>
		        </el-menu-item-group>
		        <el-menu-item-group title="分组2">
		          <el-menu-item index="2-3">选项3</el-menu-item>
		        </el-menu-item-group>
		        <el-submenu index="2-4">
		          <template slot="title">选项4</template>
		          <el-menu-item index="2-4-1">选项4-1</el-menu-item>
		        </el-submenu>
		      </el-submenu>
		      <el-submenu index="3">
		        <template slot="title"><i class="el-icon-setting"></i>导航三</template>
		        <el-menu-item-group>
		          <template slot="title">分组一</template>
		          <el-menu-item index="3-1">选项1</el-menu-item>
		          <el-menu-item index="3-2">选项2</el-menu-item>
		        </el-menu-item-group>
		        <el-menu-item-group title="分组2">
		          <el-menu-item index="3-3">选项3</el-menu-item>
		        </el-menu-item-group>
		        <el-submenu index="3-4">
		          <template slot="title">选项4</template>
		          <el-menu-item index="3-4-1">选项4-1</el-menu-item>
		        </el-submenu>
		      </el-submenu>
		    </el-menu>
		  </el-aside>
	
		   <el-container>
			     <el-header style="text-align: right; font-size: 12px">
			         <el-dropdown>
			           <i class="el-icon-setting" style="margin-right: 15px"></i>
			           <el-dropdown-menu slot="dropdown">
			             <el-dropdown-item ><el-button @click="onKeyup">退出</el-button></el-dropdown-item>
			           </el-dropdown-menu>
			         </el-dropdown>
			         <span>{{username}}</span>
			       </el-header>
		  <router-view></router-view>
		  </el-container>
		</el-container>
	</div>
</template>

	<script>
	  export default {
	    data() {
	      return {
			username:''
	      }
	    },
		methods:{
			onKeyup(){
				localStorage.clear();
					this.$router.push({ path: '/' });
			}
		},
		mounted () { //这个属性就可以，在里面声明初始化时要调用的方法即可
		      // we can implement any method here like
		      this.username=localStorage.getItem("username");
		    }
	  };
	</script>

	<style>
	  .el-header {
	    background-color: #B3C0D1;
	    color: #333;
	    line-height: 60px;
	  }
	  
	  .el-aside {
	    color: #333;
	  }
	</style>

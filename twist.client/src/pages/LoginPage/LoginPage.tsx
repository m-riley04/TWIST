const LoginPage = () => {

    return (
        <>
            <form>
                <label htmlFor="email">Email:</label><br />
                <input id="email" title="Email" type="email" placeholder="Email" /><br />
                <label htmlFor="password">Password:</label><br />
                <input id="password" title="Password" type="password" placeholder="Password" /><br />
                <button type="submit">Login</button>
            </form>
        </>
    );
}

export default LoginPage;